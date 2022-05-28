using Discord;
using Discord.WebSocket;
using Interactivity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using ScrapBot.Commands;
using ScrapBot.Entities;
using ScrapBot.Services;
using ScrapTDWrapper;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace ScrapBot
{
    public class ScrapSetup
    {
        public IConfiguration Config { get; private set; }
        public DiscordShardedClient Client { get; private set; }
        public CommandService CommandService { get; private set; }
        public ScrapClient ScrapClient { get; private set; }
        public LoggerService Logger { get; private set; }
        public IServiceProvider Provider { get; private set; }

        public async Task InitializeAsync()
        {
            var clientConfig = new DiscordSocketConfig()
            {
                AlwaysDownloadUsers = true,
                DefaultRetryMode = RetryMode.Retry502,
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Info
            };

            var commandConfig = new CommandServiceConfiguration()
            {
                DefaultRunMode = RunMode.Parallel,
                StringComparison = StringComparison.OrdinalIgnoreCase,
                IgnoresExtraArguments = true
            };

            Client = new DiscordShardedClient(clientConfig);
            CommandService = new CommandService(commandConfig);
            Config = MakeConfig();
            ScrapClient = new ScrapClient(Config["scrapApi"]);
            Provider = MakeProvider();
            Logger = Provider.GetRequiredService<LoggerService>();

            CommandService.AddModules(Assembly.GetEntryAssembly());
            InitializeTypeParsers();

            await InitializeServicesAsync();
        }

        private async Task InitializeServicesAsync()
        {
            foreach (var type in GetServiceTypes())
            {
                var service = (ScrapService)Provider.GetService(type);

                service.InjectServices(Provider);
                await service.InitializeAsync();
            }
        }

        private void InitializeTypeParsers()
        {
            CommandService.AddTypeParser(new UserParser());
            CommandService.AddTypeParser(new TextChannelParser());
            CommandService.AddTypeParser(new CommandParser(Provider));
        }

        private IServiceProvider MakeProvider()
        {
            var services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(CommandService)
                .AddSingleton(Config)
                .AddSingleton(new InteractivityService(Client,
                    new InteractivityConfig() { DefaultTimeout = TimeSpan.FromMinutes(2) }))
                .AddSingleton(ScrapClient);

            services.AddDbContext<ScrapDbContext>(options => options.UseNpgsql(Config.GetConnectionString("Remote")));

            foreach (var type in GetServiceTypes())
            {
                services.AddSingleton(type);
            }

            return services.BuildServiceProvider();
        }

        private IConfiguration MakeConfig()
            => new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("config.json")
                .Build();

        private Type[] GetServiceTypes()
            => Assembly.GetEntryAssembly().GetTypes()
                .Where(x => x.BaseType == typeof(ScrapService))
                .ToArray();

        public async Task RunAsync()
        {
            await Client.LoginAsync(TokenType.Bot, Config["tokens:bot"]);
            await ScrapClient.StartAsync();
            await Logger.LogApiCountersAsync();
            await Client.StartAsync();
            await Task.Delay(-1);
        }
    }
}

using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ScrapBot.Utils;

namespace ScrapBot.Services
{
    public sealed class LoggerService : ScrapService
    {
#pragma warning disable
        [Inject] private readonly DiscordShardedClient client;
        [Inject] private readonly IConfiguration config;
#pragma warning restore

        public override Task InitializeAsync()
        {
            client.Log += LogAsync;
            return base.InitializeAsync();
        }

        public Task LogAsync(LogMessage arg)
        {
            Console.ForegroundColor = arg.Severity switch
            {
                LogSeverity.Critical => ConsoleColor.DarkRed,
                LogSeverity.Error => ConsoleColor.Red,
                LogSeverity.Warning => ConsoleColor.Yellow,
                LogSeverity.Info => ConsoleColor.Blue,
                LogSeverity.Verbose => ConsoleColor.DarkGray,
                LogSeverity.Debug => ConsoleColor.Gray,
                _ => ConsoleColor.Green,
            };
            Console.WriteLine(arg.ToString(builder: null));
            return Task.CompletedTask;
        }

        public async Task ReportErrorAsync(SocketMessage msg, Exception ex)
        {
            var errorLogChannel = client.GetChannel(ulong.Parse(config["errorLogChannel"])) as ISocketMessageChannel;
            await errorLogChannel.SendMessageAsync(embed: EmbedUtils.Exception(msg, ex));

            await msg.Channel.SendMessageAsync(embed: EmbedUtils.Sorry);
        }
    }
}

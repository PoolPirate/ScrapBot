using System;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using ScrapBot.Utils;
using ScrapTDWrapper;

namespace ScrapBot.Services
{
    public sealed class LoggerService : ScrapService
    {
#pragma warning disable
        [Inject] private readonly DiscordShardedClient Client;
        [Inject] private readonly IConfiguration Config;
        [Inject] private readonly ScrapClient ScrapClient;
#pragma warning restore

        private readonly Timer LogApiCountersTimer;

        public LoggerService()
        {
            LogApiCountersTimer = new Timer(60000)
            {
                AutoReset = true,
                Enabled = true
            };
        }

        public override Task InitializeAsync()
        {
            Client.Log += LogAsync;
            LogApiCountersTimer.Elapsed += (e, sender) => Task.Run(() => LogApiCountersAsync());
            return LogApiCountersAsync();
        }

        public async Task LogApiCountersAsync()
        {
            var message = new LogMessage(LogSeverity.Info, "ScrapWrapper", 
                                         $"Api calls today: {ScrapClient.DailyRequestCount}, Total Api calls: {ScrapClient.TotalRequestCount}");
            await LogAsync(message);
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
            return Console.Out.WriteLineAsync(arg.ToString());
        }

        public async Task ReportErrorAsync(SocketMessage msg, Exception ex)
        {
            var errorLogChannel = Client.GetChannel(ulong.Parse(Config["errorLogChannel"])) as ISocketMessageChannel;
            await errorLogChannel.SendMessageAsync(embed: EmbedUtils.Exception(msg, ex));

            await msg.Channel.SendMessageAsync(embed: EmbedUtils.Sorry);
        }
    }
}

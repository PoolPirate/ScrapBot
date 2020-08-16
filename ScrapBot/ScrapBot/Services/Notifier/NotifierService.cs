using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScrapBot.Entities;
using ScrapBot.Utils;
using ScrapTDWrapper;

namespace ScrapBot.Services
{
    public class NotifierService : ScrapService
    {
        [Inject] private readonly DiscordShardedClient Client;
        [Inject] private readonly IServiceProvider Provider;
        [Inject] private readonly ScrapClient ScrapClient;

        private readonly Timer Updater;

        public NotifierService()
        {
            Updater = new Timer(15000)
            {
                AutoReset = false
            };

            Updater.Elapsed += async (a, b) => await UpdateNotifersAsync();
        }

        public override Task InitializeAsync()
        {
            Updater.Start();
            return Task.CompletedTask;
        }

        private async Task UpdateNotifersAsync()
        {
            var scope = Provider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ScrapDbContext>();

            var now = DateTimeOffset.UtcNow;

            var notifiers = await (dbContext.Notifiers as IQueryable<Notifier>)
                .Where(x => x.NextTrigger <= now)
                .ToListAsync();

            foreach (var notifier in notifiers)
            {
                var user = await Client.Shards.First().Rest.GetUserAsync(notifier.DiscordId);
                var channel = await user.GetOrCreateDMChannelAsync();

                notifier.NextTrigger += notifier.Interval;
                notifier.TriggerCount++;

                await SendLeaderboardsAsync(channel, notifier.Type);
            }

            await dbContext.SaveChangesAsync();
            scope.Dispose();

            Updater.Start();
        }

        private async Task SendLeaderboardsAsync(RestDMChannel channel, NotifierType type)
        {
            if (type == NotifierType.PlayerSeasonWins)
            {
                var pswLb = await ScrapClient.GetPlayerSeasonWinLeaderboardAsync();

                var page = PageUtils.LeaderBoardPage(pswLb.Take(15),
                                   0,
                                   member => member.Name,
                                   member => member.SeasonWins.ToString(),
                                   "Most Active Players",
                                   ":muscle:",
                                   EmbedColor.Leaderboard);

                await channel.SendMessageAsync(embed: page.Build().Embed);
            }
            else if (type == NotifierType.TeamSeasonWins)
            {
                var pswLb = await ScrapClient.GetTeamSeasonWinLeaderboardAsync();

                var page = PageUtils.LeaderBoardPage(pswLb.Take(15),
                                   0,
                                   teamwin => teamwin.Team.Name,
                                   teamwin => teamwin.SeasonWins.ToString(),
                                   "Most Active Teams",
                                   ":muscle:",
                                   EmbedColor.Leaderboard);

                await channel.SendMessageAsync(embed: page.Build().Embed);
            }
        }
    }
}

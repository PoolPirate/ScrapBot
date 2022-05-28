using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScrapBot.Entities;
using ScrapBot.Utils;
using ScrapTDWrapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

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

            foreach (var notifierGroup in notifiers.GroupBy(x => x.Type))
            {
                var embed = await GetNotificationEmbedAsync(notifierGroup.Key);

                foreach (var notifier in notifierGroup)
                {
                    var user = await Client.Shards.First().Rest.GetUserAsync(notifier.DiscordId);
                    var channel = await user.CreateDMChannelAsync();
                    notifier.NextTrigger += notifier.Interval;
                    notifier.TriggerCount++;

                    await channel.SendMessageAsync(embed: embed);
                }
            }

            await dbContext.SaveChangesAsync();
            scope.Dispose();

            Updater.Start();
        }

        private async Task<Embed> GetNotificationEmbedAsync(NotifierType type)
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

                return page.Build().Embed;
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

                return page.Build().Embed;
            }

            throw new InvalidOperationException("Invalid Notifier type");
        }
    }
}

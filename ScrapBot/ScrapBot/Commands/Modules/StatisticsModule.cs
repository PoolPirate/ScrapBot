using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Interactivity;
using Interactivity.Pagination;
using Interactivity.Selection;
using Qmmands;
using ScrapBot.Extensions;
using ScrapBot.Services;
using ScrapBot.Utils;
using ScrapTDWrapper;
using ScrapTDWrapper.Entities;

namespace ScrapBot.Commands
{
    [Name("Statistics :bookmark_tabs:")]
    public class StatisticsModule : ScrapModule
    {
        public InteractivityService Interactivity { get; set; }
        public LoggerService Logger { get; set; }
        public ScrapClient ScrapClient { get; set; }

        [Command("TotalWins", "TotalWin", "GlobalWins", "GlobalWin", "TW", "GW")]
        [Description("Get the total amount of matches played this season")]
        public async Task GetTotalWinsAsync()
        {
            SendConstructionMessage();

            int winCount = await ScrapClient.GetTotalSeasonWinsAsync();

            var embed = new EmbedBuilder()
                .WithColor(Color.Blue)
                .WithTitle("Total Seasonwins :muscle:")
                .WithDescription($"{winCount}")
                .Build();

            await ModifyConstructionMessageAsync(embed);
        }

        [Command("Player", "P")]
        [Description("Get statistics about a certain player")]
        [RequireBotPermission(ChannelPermission.ManageMessages | ChannelPermission.AddReactions)]
        public async Task PlayerStatisticsAsync([Name("PlayerName")][Remainder] string name)
        {
            SendConstructionMessage();

            var players = await ScrapClient.GetPlayersByNameAsync(name, 10);

            await DeleteConstructionMessageAsync();

            if (players.Length == 0)
            {
                await ReplyAsync(embed: EmbedUtils.NotFoundEmbed("Player", name));
                return;
            }
            if (players.Length == 1)
            {
                var player = players[0];
                var team = await player.GetTeamAsync();
                var member = team != null
                    ? await team.GetMemberAsync(player.Id)
                    : null;

                await ReplyAsync(embed: EmbedUtils.PlayerEmbed(player, team, member));
                return;
            }

            var sortedPlayers = players.OrderBy(x => x.Name).ToList();

            var selection = new MessageSelectionBuilder<Player>()
                                .WithValues(sortedPlayers.Take(12).ToArray())
                                .WithUsers(Context.User)
                                .WithTitle("Pick your player!")
                                .WithAllowCancel(false)
                                .WithDeletion(DeletionOptions.AfterCapturedContext | DeletionOptions.Valid)
                                .WithStringConverter(x => $"{x.Name} ({x.Trophies}:trophy:)")
                                .Build();

            var result = await Interactivity.SendSelectionAsync(selection, Context.Channel);

            if (result.IsCancelled || result.IsTimeouted)
            {
                return;
            }

            var selectedPlayer = result.Value;
            var selectedTeam = await selectedPlayer.GetTeamAsync();
            var selectedMember = selectedTeam != null
                ? await selectedTeam.GetMemberAsync(selectedPlayer.Id)
                : null;

            await ReplyAsync(embed: EmbedUtils.PlayerEmbed(selectedPlayer, selectedTeam, selectedMember));
        }

        [Command("Team", "T")]
        [Description("Get statistics about a certain team")]
        [RequireBotPermission(ChannelPermission.ManageMessages | ChannelPermission.AddReactions)]
        public async Task TeamStatisticsAsync([Name("TeamName")][Remainder] string name)
        {
            SendConstructionMessage();

            var team = await ScrapClient.GetTeamByNameAsync(name);

            await DeleteConstructionMessageAsync();

            if (team != null)
            {
                await ReplyAsync(embed: EmbedUtils.TeamEmbed(team, await team.GetLeaderAsync(), await team.GetSeasonWinsAsync()));
            }
            else
            {
                await ReplyAsync(embed: EmbedUtils.NotFoundEmbed("Team", name));
            }
        }

        [Command("Members", "Mem", "M")]
        [Description("Get all members of a certain team")]
        [RequireBotPermission(ChannelPermission.ManageMessages | ChannelPermission.AddReactions)]
        public async Task TeamMembersAsync([Name("TeamName")][Remainder] string name)
        {
            SendConstructionMessage();

            var team = await ScrapClient.GetTeamByNameAsync(name);

            await DeleteConstructionMessageAsync();

            if (team != null)
            {
                var pages = new List<PageBuilder>();

                var allMembers = await team.GetMembersAsync();
                var groups = allMembers.GroupBy(x => x.Rank);

                var leader = groups.Where(x => x.Key == Rank.Leader).First().First();
                pages.Add(PageUtils.LeaderPage(team, leader));

                if (groups.Where(x => x.Key == Rank.Coleader).Any())
                {
                    var coLeaders = groups.Where(x => x.Key == Rank.Coleader).First().OrderByDescending(x => x.Trophies).ToArray();
                    pages.AddRange(coLeaders.Chunk(10).Select(x => PageUtils.CoLeaderPage(team, x.ToArray())));
                }
                if (groups.Where(x => x.Key == Rank.Elder).Any())
                {
                    var elders = groups.Where(x => x.Key == Rank.Elder).First().OrderByDescending(x => x.Trophies).ToArray();
                    pages.AddRange(elders.Chunk(10).Select(x => PageUtils.ElderPage(team, x.ToArray())));
                }
                if (groups.Where(x => x.Key == Rank.Member).Any())
                {
                    var members = groups.Where(x => x.Key == Rank.Member).First().OrderByDescending(x => x.Trophies).ToArray();
                    pages.AddRange(members.Chunk(10).Select(x => PageUtils.MemberPage(team, x.ToArray())));
                }

                var paginator = new StaticPaginatorBuilder()
                    .WithDefaultEmotes()
                    .WithDeletion(DeletionOptions.AfterCapturedContext | DeletionOptions.Valid)
                    .WithUsers(Context.User)
                    .WithPages(pages)
                    .Build();

                await Interactivity.SendPaginatorAsync(paginator, Context.Channel);
            }
            else
            {
                await ReplyAsync(embed: EmbedUtils.NotFoundEmbed("Team", name));
            }
        }
    }
}

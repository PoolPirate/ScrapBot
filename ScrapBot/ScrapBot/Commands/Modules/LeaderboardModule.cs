using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Interactivity;
using Interactivity.Pagination;
using Qmmands;
using ScrapBot.Extensions;
using ScrapBot.Utils;
using ScrapTDWrapper;

namespace ScrapBot.Commands
{
    [Name("Leaderboards :first_place:")]
    [Group("Top", "Lb", "Leaderboard")]
    [RequireBotPermission(ChannelPermission.AddReactions | ChannelPermission.ManageMessages)]
    public class LeaderboardModule : ScrapModule
    {
        public const int PageSize = 10;

        public InteractivityService Interactivity { get; set; }
        public ScrapClient ScrapClient { get; set; }

        [Command("PlayerTrophy", "PlayerTrophies", "PlayerT", "PT")]
        [Description("Get the Player Trophy Leaderboard")]
        public async Task PlayerLeaderboardAsync()
        {
            SendConstructionMessage();
            var players = await ScrapClient.GetPlayerTrophyLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(players,
                                   player => player.Name,
                                   player => player.Trophies.ToString(),
                                   "Best Players",
                                   ":trophy:",
                                   EmbedColor.Leaderboard);
        }

        [Command("TeamTrophy", "TeamTrophies", "TeamT", "TT")]
        [Description("Get the Team Trophy Leaderboard")]
        public async Task TeamLeaderboardAsync()
        {
            SendConstructionMessage();
            var teams = await ScrapClient.GetTeamTrophyLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(teams,
                                   team => team.Name,
                                   team => team.Trophies.ToString(),
                                   "Best Teams",
                                   ":trophy:",
                                   EmbedColor.Leaderboard);
        }

        [Command("PlayerSeasonWins", "PlayerSeasonWin", "PlayerSeasonW", "PlayerSW", "PSWin", "PSW")]
        [Description("Get the Player Season Win Leaderboard")]
        public async Task PlayerwinLeaderboardAsync()
        {
            SendConstructionMessage();
            var members = await ScrapClient.GetPlayerSeasonWinLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(members,
                                   member => member.Name,
                                   member => member.SeasonWins.ToString(),
                                   "Most Active Players",
                                   ":muscle:",
                                   EmbedColor.Leaderboard);
        }

        [Command("TeamSeasonWins", "TeamSeasonWin", "TeamSeasonW", "TeamSW", "TSWin", "TSW")]
        [Description("Get the Team Season Win Leaderboard")]
        public async Task TeamwinLeaderboardAsync()
        {
            SendConstructionMessage();
            var teamwins = await ScrapClient.GetTeamSeasonWinLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(teamwins,
                                   teamwin => teamwin.Team.Name,
                                   teamwin => teamwin.SeasonWins.ToString(),
                                   "Most Active Teams",
                                   ":muscle:",
                                   EmbedColor.Leaderboard);
        }

        [Command("Level", "Lvl", "L", "Experience", "EXP", "XP", "X")]
        [Description("Get the Player Level/XP Leaderboard")]
        public async Task LevelLeaderboardAsync()
        {
            SendConstructionMessage();
            var players = await ScrapClient.GetPlayerXPLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(players,
                                   player => player.Name,
                                   player => player.Level.ToString(),
                                   "Highest Level Players",
                                   "<:crown:444526211574792192>",
                                   EmbedColor.Leaderboard);
        }

        [Command("TotalPlayerWins", "TotalPlayerW", "TPlayerWins", "TPlayerWin", "TotalWins", "TW", "TotalWin", "TotalPW", "TPW")]
        [Description("Get the Total Player Win Leaderboard")]
        public async Task PlayerTotalWinLeaderboardAsync()
        {
            SendConstructionMessage();
            var players = await ScrapClient.GetPlayerTotalWinLeaderboardAsync();
            await DeleteConstructionMessageAsync();
            await LeaderboardAsync(players,
                                   player => player.Name,
                                   player => player.TotalWins.ToString(),
                                   "Players With Most All Time Wins",
                                   ":punch:",
                                   EmbedColor.Leaderboard);
        }

        private async Task LeaderboardAsync<T>(T[] values, Func<T, string> nameSelector, Func<T, string> valueSelector,
                                               string entityName, string valueName, Color color)
        {
            var pages = new List<PageBuilder>();
            int index = 0;

            foreach (var chunk in values.Chunk(PageSize))
            {
                pages.Add(PageUtils.LeaderBoardPage(chunk, index, nameSelector, valueSelector, entityName, valueName, color));
                index += PageSize;
            }

            var paginator = new StaticPaginatorBuilder()
                                .WithDefaultEmotes()
                                .WithDeletion(DeletionOptions.Valid)
                                .WithUsers(Context.User)
                                .WithPages(pages.ToArray())
                                .Build();

            await Interactivity.SendPaginatorAsync(paginator, Context.Channel);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ScrapTDWrapper.Entities;

namespace ScrapTDWrapper
{
    public class ScrapClient
    {
        private SocketClient Client { get; }

        public int DailyRequestCount { get; private set; }
        public int TotalRequestCount { get; private set; }
        public int SessionRequestCount { get; private set; }

        public bool Started { get; private set; }

        public ScrapClient(string apiKey)
        {
            var endPoint = new IPEndPoint(IPAddress.Parse("176.9.99.146"), 34889);
            Client = new SocketClient(this, endPoint, apiKey, 180);
        }

        public async Task StartAsync()
        {
            await Client.StartAsync();
            await ResetRequestCountersAsync();
            Started = true;
        }
        public async Task ResetRequestCountersAsync()
        {
            DailyRequestCount = await Client.GetDailyUsedApiCallsAsync();
            TotalRequestCount = await Client.GetTotalUsedApiCallsAsync();
            SessionRequestCount += 2;
        }
        private void CheckClientState()
        {
            if (!Started)
            {
                throw new InvalidOperationException("Illegal attempt to access a non started ScrapClient");
            }
        }

        private void UpdateCounters()
        {
            DailyRequestCount++;
            TotalRequestCount++;
            SessionRequestCount++;
        }

        #region Native
        #region Internal
        internal Task<Team[]> GetTeamsAsync()
        {
            CheckClientState();
            UpdateCounters();
            return Client.GetTeamsAsync();
        }
        internal Task<Member[]> GetTeamMembersAsync(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId))
            {
                return Task.FromResult(Array.Empty<Member>());
            }

            CheckClientState();
            UpdateCounters();
            return Client.GetTeamMembersAsync(teamId);
        }
        internal Task<Player> GetPlayerByIdAsync(string playerId)
        {
            CheckClientState();
            UpdateCounters();
            return Client.GetPlayerByIdAsync(playerId);
        }
        internal Task<string[]> GetPlayerIdsByNameAsync(string playerName)
        {
            CheckClientState();
            UpdateCounters();
            return Client.GetPlayerIdsByNameAsync(playerName);
        }
        #endregion
        #region Public
        public async Task<TLbPlayer[]> GetPlayerTrophyLeaderboardAsync()
        {
            CheckClientState();
            UpdateCounters();
            var leaderboard = await Client.GetTrophyLeaderboardAsync();
            return leaderboard.Players;
        }
        public async Task<TLbTeam[]> GetTeamTrophyLeaderboardAsync()
        {
            CheckClientState();
            UpdateCounters();
            var leaderboard = await Client.GetTrophyLeaderboardAsync();
            return leaderboard.Teams;
        }
        public async Task<SwLbPlayer[]> GetPlayerSeasonWinLeaderboardAsync()
        {
            CheckClientState();
            UpdateCounters();
            var unfilteredLbPlayers = await Client.GetPlayerSeasonWinLeaderboardAsync();
            var filteredLbPlayers = new List<SwLbPlayer>();

            foreach (var lbPlayer in unfilteredLbPlayers)
            {
                var player = await GetPlayerByIdAsync(lbPlayer.Id);
                
                if (string.IsNullOrWhiteSpace(player.TeamId))
                {
                    continue;
                }

                filteredLbPlayers.Add(lbPlayer);
            }

            return filteredLbPlayers.ToArray();
        }
        public Task<TwLbPlayer[]> GetPlayerTotalWinLeaderboardAsync()
        {
            CheckClientState();
            UpdateCounters();
            return Client.GetPlayerTotalWinLeaderboardAsync();
        }
        public Task<XPLbPlayer[]> GetPlayerXPLeaderboardAsync()
        {
            CheckClientState();
            UpdateCounters();
            return Client.GetPlayerXPLeaderboardAsync();
        }
        #endregion
        #endregion

        #region Crafted
        #region Internal
        internal async Task<Member[]> GetAllMembersAsync()
        {
            var teams = await GetTeamsAsync();
            var members = new List<Member>();

            foreach (var team in teams)
            {
                var teamMembers = await team.GetMembersAsync();
                members.AddRange(teamMembers);
            }

            return members.ToArray();
        }
        internal async Task<Team> GetTeamByIdAsync(string teamId)
        {
            if (string.IsNullOrWhiteSpace(teamId))
            {
                return null;
            }

            var teams = await GetTeamsAsync();
            return teams.FirstOrDefault(x => x.Id == teamId);
        }
        #endregion
        #region Public
        public async Task<Player[]> GetPlayersByNameAsync(string playerName, int limit)
        {
            string[] playerIds = await GetPlayerIdsByNameAsync(playerName);

            return await Task.WhenAll(playerIds.Take(limit).Select(playerId => GetPlayerByIdAsync(playerId)));
        }
        public async Task<Team> GetTeamByNameAsync(string teamName)
        {
            var teams = await GetTeamsAsync();
            return teams.FirstOrDefault(x => x.Name.ToUpper() == teamName.ToUpper());
        }
        public async Task<(Team Team, int SeasonWins)[]> GetTeamSeasonWinLeaderboardAsync()
        {
            var teams = await GetTeamsAsync();

            var teamWins = await Task.WhenAll(teams.Select(async team =>
                           {
                               var members = await team.GetMembersAsync();
                               int seasonWins = members.Sum(x => x.SeasonWins);
                               return (Team: team, SeasonWins: seasonWins);
                           }));

            return teamWins.OrderByDescending(x => x.SeasonWins).ToArray();
        }
        public async Task<int> GetTotalSeasonWinsAsync()
        {
            var members = await GetAllMembersAsync();
            return members.Sum(x => x.SeasonWins);
        }
        #endregion
        #endregion
    }
}


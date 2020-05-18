using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ScrapTDWrapper.Entities;

namespace ScrapTDWrapper
{
    internal class SocketClient
    {
        private ScrapClient ScrapClient { get; set; }
        public TcpClient Client { get; private set; }
        public IPEndPoint EndPoint { get; }
        public string ApiKey { get; }

        private SemaphoreSlim RequestSemaphore { get; }
        private StreamWriter Writer { get; set; }
        private StreamReader Reader { get; set; }
        private NetworkStream Stream { get; set; }

        private int KeepAliveTime { get; set; }
        private System.Timers.Timer KeepAliveTimer { get; set; }

        public SocketClient(ScrapClient scrapClient, IPEndPoint endPoint, string apiKey, int keepAliveSeconds)
        {
            ScrapClient = scrapClient;
            Client = new TcpClient();
            EndPoint = endPoint;
            RequestSemaphore = new SemaphoreSlim(1, 1);
            KeepAliveTime = keepAliveSeconds * 1000;

            ApiKey = !apiKey.StartsWith("api:")
                ? $"api:{apiKey}"
                : apiKey;
        }

        public async Task StartAsync() => await ConnectAsync();
        private async Task ConnectAsync()
        {
            if (!Client.Connected)
            {
                Client = new TcpClient();
                await Client.ConnectAsync(EndPoint.Address, EndPoint.Port);

                Stream = Client.GetStream();
                Writer = new StreamWriter(Stream);
                Reader = new StreamReader(Stream);
                KeepAliveTimer = new System.Timers.Timer(KeepAliveTime)
                {
                    AutoReset = true,
                };
                KeepAliveTimer.Elapsed += (e, sender) => Task.Run(() => SendHeartbeatAsync());

                Writer.NewLine = "\n";
                Writer.AutoFlush = true;

                await Writer.WriteLineAsync(ApiKey);

                await CheckServerResponseAsync();

                KeepAliveTimer.Start();
            }
        }
        private void Disconnect()
        {
            Client.Close();
            Client.Dispose();
            Reader.Dispose();
            Writer.Dispose();
            Stream.Dispose();

            KeepAliveTimer.Dispose();
        }
        private async Task SendHeartbeatAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("keepalive");
                await CheckServerResponseAsync(); 
                await CheckServerResponseAsync();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        private async Task CheckServerResponseAsync()
        {
            await WaitForDataAsync(7500);
            string response = await Reader.ReadLineAsync();

            switch (response)
            {
                case "AA":
                    break;
                case "EE":
                    throw new Exception("The server responded with EE (Error)!");
                case "LL":
                    throw new Exception("The server responded with LL (Rate Limit Reached)");
                default:
                    throw new Exception($"Something went wrong! The server response was {response}");
            }
        }
        private async Task WaitForDataAsync(int timeout)
        {
            int elapsed = 0;

            while (!Stream.DataAvailable)
            {
                await Task.Delay(10);
                elapsed += 10;

                if (elapsed > timeout)
                {
                    Disconnect();
                    throw new Exception("No data received!");
                }
            }
        }
        private async Task CheckConnectionStatusAsync()
        {
            if (!Client.Connected)
            {
                Disconnect();
                await ConnectAsync();
            }
            else
            {
            }

            if (!Client.Connected)
            {
                throw new Exception("The client is not connected and failed reconnecting!");
            }
        }

        #region Requests
        public async Task<int> GetTotalUsedApiCallsAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("totalapirequests");

                await CheckServerResponseAsync();

                string response = await Reader.ReadLineAsync();

                return int.Parse(response);
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<int> GetDailyUsedApiCallsAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("apirequests");

                await CheckServerResponseAsync();

                string response = await Reader.ReadLineAsync();

                return int.Parse(response);
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<Team[]> GetTeamsAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("getteams");

                await CheckServerResponseAsync();

                string response = await Reader.ReadLineAsync();

                string[] teamStrings = response.Split(';');
                return teamStrings.Select(teamString => Team.Parse(ScrapClient, teamString))
                                  .ToArray();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<Member[]> GetTeamMembersAsync(string teamId)
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync($"getteambyid:{teamId}");

                await CheckServerResponseAsync();

                string response = await Reader.ReadLineAsync();

                string[] memberStrings = response.Split(';');
                return memberStrings.Select(memberString => Member.Parse(ScrapClient, memberString))
                                    .ToArray();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<Player> GetPlayerByIdAsync(string playerId)
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync($"getplayerdata:{playerId}");

                await CheckServerResponseAsync();

                string playerString = await Reader.ReadLineAsync();

                return Player.Parse(ScrapClient, playerString);
            }
            finally
            {
                RequestSemaphore.Release();
            }

        }

        public async Task<string[]> GetPlayerIdsByNameAsync(string playerName)
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync($"getplayersbyname:{Formatter.ToFormatted(playerName)}");

                await CheckServerResponseAsync();

                string response = await Reader.ReadLineAsync();

                string[] playerIds = response.Split(':', StringSplitOptions.RemoveEmptyEntries);
                return playerIds;
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<TLeaderboard> GetTrophyLeaderboardAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("getleaderboard");

                await CheckServerResponseAsync();

                string leaderboardString = await Reader.ReadLineAsync();

                string[] parts = leaderboardString.Split(':');
                string[] playerStrings = parts[0].Split(';');
                string[] teamStrings = parts[1].Split(";");

                var players = playerStrings.Select(playerString => TLbPlayer.Parse(ScrapClient, playerString))
                                           .ToArray();

                var teams = teamStrings.Select(teamString => TLbTeam.Parse(ScrapClient, teamString))
                                       .ToArray();

                return new TLeaderboard(players, teams);
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<SwLbPlayer[]> GetPlayerSeasonWinLeaderboardAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("seasonwinleaderboard");

                await CheckServerResponseAsync();

                string leaderboardString = await Reader.ReadLineAsync();

                string[] swLbPlayerStrings = leaderboardString.Split(";");
                return swLbPlayerStrings.Select(swLbPlayerString => SwLbPlayer.Parse(ScrapClient, swLbPlayerString))
                                        .ToArray();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<TwLbPlayer[]> GetPlayerTotalWinLeaderboardAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("totalwinleaderboard");

                await CheckServerResponseAsync();

                string leaderboardString = await Reader.ReadLineAsync();

                string[] twLbPlayerStrings = leaderboardString.Split(";");
                return twLbPlayerStrings.Select(twLbPlayerString => TwLbPlayer.Parse(ScrapClient, twLbPlayerString))
                                        .ToArray();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }

        public async Task<XPLbPlayer[]> GetPlayerXPLeaderboardAsync()
        {
            try
            {
                await RequestSemaphore.WaitAsync();

                await CheckConnectionStatusAsync();

                await Writer.WriteLineAsync("xpleaderboard");

                await CheckServerResponseAsync();

                string leaderboardString = await Reader.ReadLineAsync();

                string[] xpLbPlayerStrings = leaderboardString.Split(";");
                return xpLbPlayerStrings.Select(xpLbPlayerString => XPLbPlayer.Parse(ScrapClient, xpLbPlayerString))
                                        .ToArray();
            }
            finally
            {
                RequestSemaphore.Release();
            }
        }
        #endregion
    }
}

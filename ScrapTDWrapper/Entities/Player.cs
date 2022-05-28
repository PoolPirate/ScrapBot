using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapTDWrapper.Entities
{
    public class Player : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public Rank Rank { get; }
        public int Trophies { get; }
        public string TeamId { get; }
        public int MatchesWon { get; }
        public int MaxTrophies { get; }
        public int Level { get; }

        private Player(ScrapClient client, string id, string name, Rank rank, int trophies, string teamId, int matchesWon, int maxTrophies, int level)
            : base(client)
        {
            Id = id;
            Name = name;
            Rank = rank;
            Trophies = trophies;
            TeamId = teamId;
            MatchesWon = matchesWon;
            MaxTrophies = maxTrophies;
            Level = level;
        }

        internal static Player Parse(ScrapClient client, string playerString)
        {
            string[] parts = playerString.Split(':')
                                         .Select(x => Formatter.FromFormatted(x))
                                         .ToArray();

            return new Player(client,
                              parts[0],
                              parts[1],
                              (Rank)Enum.Parse(typeof(Rank), parts[2]),
                              Int32.Parse(parts[3]),
                              parts[4],
                              Int32.Parse(parts[5]),
                              Int32.Parse(parts[6]),
                              Int32.Parse(parts[7]));
        }

        public async Task<Team> GetTeamAsync()
            => TeamId == ScrapClient.EmptyTeamId
            ? null
            : await Client.GetTeamByIdAsync(TeamId);

    }
}

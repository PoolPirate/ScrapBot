using System.Linq;
using System.Threading.Tasks;

namespace ScrapTDWrapper.Entities
{
    public class Team : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public string Icon { get; }
        public string Description { get; }
        public int PlayerCount { get; }
        public int Trophies { get; }
        public int RequiredTrophies { get; }


        private Team(ScrapClient client, string id, string name, string icon, string description, int playerCount, int trophies, int requiredTrophies)
            : base(client)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Description = description;
            PlayerCount = playerCount;
            Trophies = trophies;
            RequiredTrophies = requiredTrophies;
        }
        internal static Team Parse(ScrapClient client, string teamString)
        {
            string[] parts = teamString.Split(':')
                                       .Select(x => Formatter.FromFormatted(x))
                                       .ToArray();

            return new Team(client,
                            parts[0],
                            parts[1],
                            parts[2],
                            parts[3],
                            int.Parse(parts[4]),
                            int.Parse(parts[5]),
                            int.Parse(parts[6]));
        }

        public Task<Member[]> GetMembersAsync() => Client.GetTeamMembersAsync(Id);
        public async Task<Member> GetMemberAsync(string playerId)
        {
            var members = await GetMembersAsync();
            return members.FirstOrDefault(x => x.Id == playerId);
        }
        public async Task<int> GetSeasonWinsAsync()
        {
            var members = await GetMembersAsync();
            return members.Sum(x => x.SeasonWins);
        }
        public async Task<Member> GetLeaderAsync()
        {
            var members = await GetMembersAsync();
            return members.First(x => x.Rank == Rank.Leader);
        }
    }
}

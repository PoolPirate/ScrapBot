using System;
using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public sealed class Member : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public Rank Rank { get; }
        public int Trophies { get; }
        public int Level { get; }
        public bool IsActive { get; }
        public int SeasonWins { get; }

        private Member(ScrapClient client, string id, string name, Rank rank, int trophies, int level, bool isActive, int seasonWins)
            : base(client)
        {
            Id = id;
            Name = name;
            Rank = rank;
            Trophies = trophies;
            Level = level;
            IsActive = isActive;
            SeasonWins = seasonWins;
        }

        internal static Member Parse(ScrapClient client, string memberString)
        {
            string[] parts = memberString.Split(':')
                                         .Select(x => Formatter.FromFormatted(x))
                                         .ToArray();

            return new Member(client,
                              parts[0],
                              parts[1],
                              (Rank) Enum.Parse(typeof(Rank), parts[2]),
                              int.Parse(parts[3]),
                              int.Parse(parts[4]),
                              parts[5] == "0",
                              int.Parse(parts[6]));
        }
    }
}

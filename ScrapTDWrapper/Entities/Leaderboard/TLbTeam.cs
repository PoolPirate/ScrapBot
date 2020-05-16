using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public sealed class TLbTeam : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public int Trophies { get; }
        public int Icon { get; }
        public int PlayerCount { get; }

        private TLbTeam(ScrapClient client, string id, string name, int trophies, int icon, int playerCount)
            : base(client)
        {
            Id = id;
            Name = name;
            Trophies = trophies;
            Icon = icon;
            PlayerCount = playerCount;
        }

        internal static TLbTeam Parse(ScrapClient client, string teamString)
        {
            string[] parts = teamString.Split(',')
                                        .Select(x => Formatter.FromFormatted(x))
                                        .ToArray();

            return new TLbTeam(client,
                               parts[0],
                               parts[1],
                               int.Parse(parts[2]),
                               int.Parse(parts[3]),
                               int.Parse(parts[4]));
        }
    }
}

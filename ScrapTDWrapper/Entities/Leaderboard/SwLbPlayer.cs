using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public class SwLbPlayer : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public int SeasonWins { get; }

        private SwLbPlayer(ScrapClient client, string id, string name, int seasonWins)
            : base(client)
        {
            Id = id;
            Name = name;
            SeasonWins = seasonWins;
        }

        internal static SwLbPlayer Parse(ScrapClient client, string swLbPlayerString)
        {
            string[] parts = swLbPlayerString.Split(':')
                                             .Select(x => Formatter.FromFormatted(x))
                                             .ToArray();

            return new SwLbPlayer(client,
                                  parts[0],
                                  parts[1],
                                  int.Parse(parts[2]));
        }
    }
}

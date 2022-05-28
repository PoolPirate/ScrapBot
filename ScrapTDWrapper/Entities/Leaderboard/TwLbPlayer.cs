using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public class TwLbPlayer : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public int TotalWins { get; }

        private TwLbPlayer(ScrapClient client, string id, string name, int totalWins)
            : base(client)
        {
            Id = id;
            Name = name;
            TotalWins = totalWins;
        }

        internal static TwLbPlayer Parse(ScrapClient client, string twLbPlayerString)
        {
            string[] parts = twLbPlayerString.Split(':')
                                             .Select(x => Formatter.FromFormatted(x))
                                             .ToArray();

            return new TwLbPlayer(client,
                                  parts[0],
                                  parts[1],
                                  System.Int32.Parse(parts[2]));
        }
    }
}

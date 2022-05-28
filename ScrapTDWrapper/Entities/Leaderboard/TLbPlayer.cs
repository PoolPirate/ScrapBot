using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public sealed class TLbPlayer : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public int Trophies { get; }
        public int Level { get; }

        internal TLbPlayer(ScrapClient client, string id, string name, int trophies, int level)
            : base(client)
        {
            Id = id;
            Name = name;
            Trophies = trophies;
            Level = level;
        }

        internal static TLbPlayer Parse(ScrapClient client, string playerString)
        {
            string[] parts = playerString.Split(',')
                                         .Select(x => Formatter.FromFormatted(x))
                                         .ToArray();

            return new TLbPlayer(client,
                                 parts[0],
                                 parts[1],
                                 System.Int32.Parse(parts[2]),
                                 System.Int32.Parse(parts[3]));
        }
    }
}

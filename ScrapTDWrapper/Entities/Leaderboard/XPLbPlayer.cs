using System.Globalization;
using System.Linq;

namespace ScrapTDWrapper.Entities
{
    public class XPLbPlayer : SocketEntity
    {
        public string Id { get; }
        public string Name { get; }
        public double Level { get; }

        private XPLbPlayer(ScrapClient client, string id, string name, double level)
            : base(client)
        {
            Id = id;
            Name = name;
            Level = level;
        }

        internal static XPLbPlayer Parse(ScrapClient client, string xpLbPlayerString)
        {
            string[] parts = xpLbPlayerString.Split(':')
                                             .Select(x => Formatter.FromFormatted(x))
                                             .ToArray();

            return new XPLbPlayer(client,
                                  parts[0],
                                  parts[1],
                                  System.Double.Parse(parts[2], CultureInfo.InvariantCulture));
        }
    }
}

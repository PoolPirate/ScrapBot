using System;

namespace ScrapBot.Entities
{
    public class Notifier
    {
        public Guid Id { get; private set; }
        public ulong DiscordId { get; private set; }

        public DateTimeOffset NextTrigger { get; set; }
        public TimeSpan Interval { get; private set; }
        public int TriggerCount { get; set; }

        public bool CompactDisplay { get; private set; }
        public NotifierType Type { get; private set; }


        public Notifier(ulong discordId, bool compactDisplay, NotifierType type, TimeSpan interval)
        {
            Id = Guid.NewGuid();

            DiscordId = discordId;
            CompactDisplay = compactDisplay;
            Type = type;
            Interval = interval;
        }
    }
}

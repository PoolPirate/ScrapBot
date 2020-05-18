using System;
using Discord.WebSocket;
using Qmmands;

namespace ScrapBot.Commands
{
    public class ScrapContext : CommandContext
    {
        public SocketTextChannel Channel { get; }
        public DiscordShardedClient Client { get; }
        public SocketGuild Guild { get; }
        public SocketUserMessage Message { get; }
        public SocketUser User { get; }

        public ScrapContext(DiscordShardedClient client, SocketUserMessage msg, IServiceProvider provider)
            : base(provider)
        {
            Client = client;
            Channel = msg.Channel as SocketTextChannel;
            Guild = Channel.Guild;
            User = msg.Author;
            Message = msg;
        }
    }
}

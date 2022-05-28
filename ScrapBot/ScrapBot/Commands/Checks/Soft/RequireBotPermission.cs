using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [Description("Requires the bot to have certain permissions")]
    public class RequireBotPermissionAttribute : SoftCheckAttribute
    {
        public GuildPermission? GuildPermission { get; }
        public ChannelPermission? ChannelPermission { get; }

        public RequireBotPermissionAttribute(GuildPermission permission)
        {
            GuildPermission = permission;
            ChannelPermission = null;
        }

        public RequireBotPermissionAttribute(ChannelPermission permission)
        {
            ChannelPermission = permission;
            GuildPermission = null;
        }

        public override ValueTask<CheckResult> CheckAsync(ScrapContext context)
        {
            IGuildUser guildUser = context.Guild.CurrentUser;

            if (GuildPermission.HasValue)
            {
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                {
                    return CheckResult.Unsuccessful($"I'm missing the {GuildPermission.Value} guild permission :scream:");
                }
            }

            if (ChannelPermission.HasValue)
            {
                IGuildChannel guildChannel = context.Channel;
                var perms = guildUser.GetPermissions(guildChannel);

                if (!perms.Has(ChannelPermission.Value))
                {
                    return CheckResult.Unsuccessful($"I'm missing the {ChannelPermission.Value} channel permission :scream:");
                }
            }

            return CheckResult.Successful;
        }
    }
}

using Discord;
using Discord.WebSocket;
using Qmmands;
using ScrapBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public sealed class UserParser : ScrapTypeParser<IUser>
    {
        public override ValueTask<TypeParserResult<IUser>> ParseAsync(Parameter param, string value, ScrapContext context)
        {
            var users = context.Guild.Users.OfType<IUser>().ToList();
            IUser user = null;

            if (System.UInt64.TryParse(value, out ulong id) || MentionUtils.TryParseUser(value, out id))
            {
                user = context.Client.GetUser(id);
            }

            if (user is null)
            {
                var match = users.Where(x =>
                    x.Username.EqualsIgnoreCase(value)
                    || (x as SocketGuildUser).Nickname.EqualsIgnoreCase(value)).ToList();
                if (match.Count() > 1)
                {
                    return TypeParserResult<IUser>.Unsuccessful(
                        "Multiple users found, try mentioning the user or using their ID.");
                }

                user = match.FirstOrDefault();
            }
            return user is null
                ? TypeParserResult<IUser>.Unsuccessful("User not found.")
                : TypeParserResult<IUser>.Successful(user);
        }
    }

}

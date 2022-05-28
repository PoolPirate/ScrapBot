using Discord;
using Qmmands;
using ScrapBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public sealed class RoleParser : ScrapTypeParser<IRole>
    {
        public override ValueTask<TypeParserResult<IRole>> ParseAsync(Parameter param, string value, ScrapContext context)
        {
            var roles = context.Guild.Roles.ToList();
            IRole role = null;

            if (System.UInt64.TryParse(value, out ulong id) || MentionUtils.TryParseRole(value, out id))
            {
                role = context.Guild.GetRole(id);
            }

            if (role is null)
            {
                var match = roles.Where(x =>
                    x.Name.EqualsIgnoreCase(value));
                if (match.Count() > 1)
                {
                    return TypeParserResult<IRole>.Unsuccessful(
                        "Multiple roles found, try mentioning the role or using its ID.");
                }

                role = match.FirstOrDefault();
            }
            return role is null
                ? TypeParserResult<IRole>.Unsuccessful("Role not found.")
                : TypeParserResult<IRole>.Successful(role);
        }
    }
}

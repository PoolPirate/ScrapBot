using Discord;
using Qmmands;
using ScrapBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public sealed class TextChannelParser : ScrapTypeParser<ITextChannel>
    {
        public override ValueTask<TypeParserResult<ITextChannel>> ParseAsync(Parameter param, string value, ScrapContext context)
        {
            var channels = context.Guild.Channels.OfType<ITextChannel>().ToList();
            ITextChannel channel = null;

            if (System.UInt64.TryParse(value, out ulong id) || MentionUtils.TryParseChannel(value, out id))
            {
                channel = context.Client.GetChannel(id) as ITextChannel;
            }

            if (channel is null)
            {
                var match = channels.Where(x =>
                    x.Name.EqualsIgnoreCase(value));
                if (match.Count() > 1)
                {
                    return TypeParserResult<ITextChannel>.Failed(
                        "Multiple channels found, try mentioning the channel or using its ID.");
                }

                channel = match.FirstOrDefault();
            }
            return channel is null
                ? TypeParserResult<ITextChannel>.Failed("User not found.")
                : TypeParserResult<ITextChannel>.Successful(channel);
        }
    }
}

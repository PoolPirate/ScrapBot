﻿using Discord;
using Qmmands;
using ScrapBot.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public class MessageParser : ScrapTypeParser<IMessage>
    {
        public override async ValueTask<TypeParserResult<IMessage>> ParseAsync(Parameter parameter, string value, ScrapContext context)
        {
            var messages = context.Channel.CachedMessages;
            IMessage message = null;

            if (System.UInt64.TryParse(value, out ulong id))
            {
                message = await context.Channel.GetMessageAsync(id);
            }

            if (message is null)
            {
                var match = messages.Where(x =>
                    x.Content.EqualsIgnoreCase(value));
                if (match.Count() > 1)
                {
                    return TypeParserResult<IMessage>.Unsuccessful(
                        "Multiple messages found, try using its ID.");
                }

                message = match.FirstOrDefault();
            }
            return message is null
                ? TypeParserResult<IMessage>.Unsuccessful("Message not found.")
                : TypeParserResult<IMessage>.Successful(message);
        }
    }
}

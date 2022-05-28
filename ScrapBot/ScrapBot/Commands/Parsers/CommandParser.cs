using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public class CommandParser : ScrapTypeParser<Command>
    {
        private readonly IServiceProvider Provider;

        public CommandParser(IServiceProvider provider)
        {
            Provider = provider;
        }

        public override ValueTask<TypeParserResult<Command>> ParseAsync(Parameter parameter, string value, ScrapContext context)
        {
            var _commands = Provider.GetService<CommandService>();
            var matchingCommands = _commands.FindCommands(value);

            return matchingCommands.Count == 0
                ? TypeParserResult<Command>.Failed("Could not find a command matching your input!")
                : TypeParserResult<Command>.Successful(matchingCommands[0].Command);
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public class CommandParser : ScrapTypeParser<Command>
    {
        public override ValueTask<TypeParserResult<Command>> ParseAsync(Parameter parameter, string value, ScrapContext context)
        {
            var _commands = context.ServiceProvider.GetService<CommandService>();
            var matchingCommands = _commands.FindCommands(value);

            return matchingCommands.Count == 0
                ? TypeParserResult<Command>.Unsuccessful("Could not find a command matching your input!")
                : TypeParserResult<Command>.Successful(matchingCommands[0].Command);
        }
    }
}

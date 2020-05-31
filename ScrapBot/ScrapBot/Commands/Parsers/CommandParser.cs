using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace ScrapBot.Commands
{
    public class CommandParser : ScrapTypeParser<Command>
    {
        public override ValueTask<TypeParserResult<Command>> ParseAsync(Parameter parameter, string value, ScrapContext context)
        {
            var _commands = context.ServiceProvider.GetService<CommandService>();

            var command = _commands.FindCommands(value).FirstOrDefault()?.Command;

            return command == null
                ? TypeParserResult<Command>.Unsuccessful("Could not find a command matching your input!")
                : TypeParserResult<Command>.Successful(command);
        }
    }
}

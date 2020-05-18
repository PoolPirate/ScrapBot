using System.Threading.Tasks;
using Qmmands;

namespace ScrapBot.Commands
{
    public abstract class ScrapTypeParser<T> : TypeParser<T>
    {
        public override ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, CommandContext context)
            => ParseAsync(parameter, value, context as ScrapContext);

        public abstract ValueTask<TypeParserResult<T>> ParseAsync(Parameter parameter, string value, ScrapContext context);
    }
}

using System.Reflection;
using System.Threading.Tasks;
using Qmmands;

namespace ScrapBot.Commands
{
    public abstract class ScrapCheckAttribute : CheckAttribute
    {
        public string Description { get; set; }

        public ScrapCheckAttribute()
        {
            Description = GetType().GetCustomAttribute<DescriptionAttribute>().Value;
        }

        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        => CheckAsync(context as ScapContext);

        public abstract ValueTask<CheckResult> CheckAsync(ScapContext context);
    }
}

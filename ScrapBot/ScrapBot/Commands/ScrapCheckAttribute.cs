using Qmmands;
using System.Reflection;
using System.Threading.Tasks;

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
        => CheckAsync(context as ScrapContext);

        public abstract ValueTask<CheckResult> CheckAsync(ScrapContext context);
    }
}

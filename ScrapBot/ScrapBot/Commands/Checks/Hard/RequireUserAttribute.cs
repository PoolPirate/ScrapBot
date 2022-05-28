using Qmmands;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    [Description("Requires a specific group of users")]
    public class RequireUserAttribute : HardCheckAttribute
    {
        public ulong[] UserIds { get; set; }

        public RequireUserAttribute(params ulong[] userIds)
        {
            UserIds = userIds;
        }

        public override ValueTask<CheckResult> CheckAsync(ScrapContext context)
            => UserIds.Contains(context.User.Id)
                ? CheckResult.Successful
                : CheckResult.Failed("You do not have access to this command!");
    }
}

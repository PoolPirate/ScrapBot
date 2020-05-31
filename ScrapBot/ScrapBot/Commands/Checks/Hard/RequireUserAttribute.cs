using System.Linq;
using System.Threading.Tasks;
using Qmmands;

namespace ScrapBot.Commands
{
    public class RequireUserAttribute : HardCheckAttribute
    {
        public ulong[] UserIds { get; set; }

        public RequireUserAttribute(params ulong[] userIds)
        {
            UserIds = userIds;
        }

        public override ValueTask<CheckResult> CheckAsync(ScrapContext context)
        {
            return UserIds.Contains(context.User.Id)
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("You do not have access to this command!");
        }
    }
}

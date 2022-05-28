using Qmmands;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    [Description("Requires the bot owner to run the command")]
    public class RequireOwnerAttribute : HardCheckAttribute
    {
        public override async ValueTask<CheckResult> CheckAsync(ScrapContext context)
        {
            var appInfo = await context.Client.GetApplicationInfoAsync();

            return appInfo.Owner.Id != context.User.Id
                ? CheckResult.Unsuccessful("This command is restricted to the Owner!")
                : CheckResult.Successful;
        }
    }
}

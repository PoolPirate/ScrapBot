using Qmmands;
using ScrapBot.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Extensions
{
    public static partial class Extensions
    {
        public static async Task<bool> RunHardChecksAsync(this Module module, ScrapContext context)
        {
            var checks = module.Checks
                .Where(x => x is HardCheckAttribute)
                .Select(x => x as HardCheckAttribute).ToList();

            var checkResults = await Task.WhenAll(checks.Select(x => x.CheckAsync(context).AsTask()));

            return !checkResults.Any(x => !x.IsSuccessful);
        }

        public static async Task<bool> RunSoftChecksAsync(this Module module, ScrapContext context)
        {
            var checks = module.Checks
                .Where(x => x is SoftCheckAttribute)
                .Select(x => x as SoftCheckAttribute).ToList();

            var checkResults = await Task.WhenAll(checks.Select(x => x.CheckAsync(context).AsTask()));

            return checkResults.Any(x => !x.IsSuccessful);
        }
    }
}

using System.Threading.Tasks;

namespace ScrapBot
{
    public class Program
    {
        private static ScrapSetup Bot;

#pragma warning disable
        private static void Main(string[] args)
#pragma warning restore
        {
            Bot = new ScrapSetup();

            Task.Run(async () =>
            {
                await Bot.InitializeAsync();
                await Bot.RunAsync();
            })
            .GetAwaiter().GetResult();
        }

    }
}

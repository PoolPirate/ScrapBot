using Discord;
using Qmmands;
using ScrapBot.Utils;
using System.Threading.Tasks;

namespace ScrapBot.Commands.Modules
{
    [RequireOwner]
    public class OwnerModule : ScrapModule
    {
        [Command("ban")]
        [RequireOwner]
        public async Task FakeBanAsync(IUser user)
        {
            var embed = new EmbedBuilder()
                .WithColor(EmbedColor.Success)
                .WithTitle("Success")
                .WithDescription($"{user} has been banned!\n" +
                $"Thanks for making this discord server a little bit safer :)")
                .Build();

            await ReplyAsync(embed: embed);
        }
    }
}

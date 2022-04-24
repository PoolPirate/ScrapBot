using System.Threading.Tasks;
using Discord;
using Discord.Rest;
using Qmmands;
using ScrapBot.Utils;

namespace ScrapBot.Commands
{
    public abstract class ScrapModule : ModuleBase<ScrapContext>
    {
        private Task<RestUserMessage> ConstructionMessageTask { get; set; }

        public async Task<RestUserMessage> ReplyAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await Context.Channel.SendMessageAsync(text, isTTS, embed, options);

        public async Task<IUserMessage> SendConstructionMessage()
            => await ReplyAsync(embed: EmbedUtils.ConstructionEmbed);

        public async Task ModifyConstructionMessageAsync(Embed embed)
            => await (await ConstructionMessageTask).ModifyAsync(x => x.Embed = embed);
    }
}

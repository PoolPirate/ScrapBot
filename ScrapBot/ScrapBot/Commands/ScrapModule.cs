﻿using Discord;
using Discord.Rest;
using Qmmands;
using ScrapBot.Utils;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    public abstract class ScrapModule : ModuleBase<ScrapContext>
    {
        public async Task<RestUserMessage> ReplyAsync(string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
            => await Context.Channel.SendMessageAsync(text, isTTS, embed, options);

        public async Task<IUserMessage> SendConstructionMessage()
            => await ReplyAsync(embed: EmbedUtils.ConstructionEmbed);
    }
}

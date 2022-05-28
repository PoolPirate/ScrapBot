﻿using Discord;
using Qmmands;
using ScrapBot.Extensions;
using ScrapBot.Utils;
using ScrapTDWrapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrapBot.Commands
{
    [Name("Information :newspaper:")]
    public class InfoModule : ScrapModule
    {
        public ScrapClient ScrapClient { get; set; }
        public CommandService Command { get; set; }

        [Command("Bots", "Matches", "Match")]
        [Description("Tutorial on how to use Bots")]
        public Task BotEmotesAsync()
            => ReplyAsync(embed: EmbedUtils.Bots);

        [Command("ApiInfo", "ApiCalls", "ApiRequests", "Api")]
        [Description("Gets some information about the underlying API")]
        public Task ApiInfoAsync()
            => ReplyAsync(embed: EmbedUtils.ApiInfo(ScrapClient.SessionRequestCount, ScrapClient.DailyRequestCount, ScrapClient.TotalRequestCount));

        [Command("ReloadCounters", "UpdateCounters")]
        [Description("Reloads the counters of the internal ScrapClient")]
        [RequireOwner]
        public async Task ReloadCountersAsync()
        {
            await ScrapClient.ResetRequestCountersAsync();
            await ReplyAsync(embed: EmbedUtils.ReloadComplete);
        }

        [Command("Invite")]
        [Description("Take a look at the credits and the bot invite link")]
        public Task InviteAsync()
            => ReplyAsync(embed: EmbedUtils.Invite);

        [Command("CommandHelp", "CommandInfo", "CmdInfo", "Help", "CmdHelp", "CInfo", "CHelp", "HelpMe", "HowDoesThisWork")]
        [Description("Get help about a certain command")]
        [Priority(1)]
        public Task SendCommandHelpAsync([Name("Command")][Remainder] Command command)
            => ReplyAsync(embed: command.GetHelpEmbed());

        [Command("Help", "HelpMe", "HowDoesThisWork")]
        [Description("Get the entire help page")]
        public async Task SendHelpAsync()
        {
            var helpBuilder = new EmbedBuilder();

            foreach (var module in Command.GetAllModules())
            {
                if (!await module.RunHardChecksAsync(Context))
                {
                    continue;
                }

                var runnableCommands = new List<Command>();

                foreach (var command in module.Commands)
                {
                    if (!await command.RunHardChecksAsync(Context))
                    {
                        continue;
                    }

                    runnableCommands.Add(command);
                }

                if (runnableCommands.Count == 0)
                {
                    continue;
                }

                helpBuilder.AddField($"{module.Name}",
                                     $"{System.String.Join("\n", runnableCommands.Select(x => x.GetUsageMessage()))}");
            }

            var helpEmbed = helpBuilder
                .WithTitle("Help Page")
                .WithColor(EmbedColor.Help)
                .Build();

            await ReplyAsync(embed: helpEmbed);
        }
    }
}

using Discord;
using Discord.WebSocket;
using Qmmands;
using ScrapBot.Extensions;
using ScrapTDWrapper.Entities;
using System;
using System.Linq;
using System.Text;

namespace ScrapBot.Utils
{
    public static class EmbedUtils
    {
        public static Embed Bots
            => new EmbedBuilder()
                .WithColor(EmbedColor.Info)
                .WithTitle("Bot Usage in ScrapTD")
                .WithDescription("#1 Select the Emotes shown in the image below in your emote Tab\n" +
                                 "#2 Press play and wait about 10 seconds\n" +
                                 "#3 Try to stay alive, most bots usually stop at some point :P")
                .WithImageUrl("https://cdn.discordapp.com/attachments/441308951221370920/718940472080466061/Screenshot_20200606-233233_ScrapTD.jpg")
                .Build();

        public static Embed Invite
            => new EmbedBuilder()
                .WithColor(EmbedColor.Info)
                .WithTitle("Invites")
                .AddField("Add me to your server", "https://discord.com/oauth2/authorize?client_id=512218279071186955&scope=bot&permissions=67202048")
                .AddField("Official Scrap Discord", "https://discord.gg/xu9NUcv")
                .AddField("ScrapTD Community Discord", "https://discord.gg/pEYP3KZ")
                .AddField("Suggestions / Bug Reports", "https://discord.gg/YzEuqpk")
                .Build();

        public static Embed ReloadComplete
            => new EmbedBuilder()
                .WithColor(EmbedColor.Info)
                .WithTitle("Reload Completed!")
                .Build();

        public static Embed ApiInfo(int sessionApiCalls, int dailyApiCalls, int totalApiCalls)
            => new EmbedBuilder()
                .WithColor(EmbedColor.Info)
                .WithTitle("ScrapTD API Info")
                .AddField("Call Counter", $"- Session: {sessionApiCalls}\n- Day: {dailyApiCalls}\n- Total: {totalApiCalls}")
                .Build();

        public static Embed Exception(SocketMessage msg, Exception ex)
            => new EmbedBuilder()
                .WithColor(EmbedColor.Error)
                .WithTitle("ERROR, FIX ME NOW!")
                .WithDescription($"Stacktrace:\n{ex.Message}\n{ex.StackTrace}".Truncate(2047))
                .AddField("Additional Infos:", $"Message: {msg.Content}\nUser: {msg.Author}\nChannel: {msg.Channel}")
                .Build();

        public static Embed Sorry
            => new EmbedBuilder()
                .WithColor(EmbedColor.Error)
                .WithTitle("An error occured!")
                .WithDescription("Please don't repeat whatever you did.\n" +
                                 "The error has already been reported and will most likely be fixed soon :)")
                .Build();

        public static Embed UnsupportedEnvironment =>
            new EmbedBuilder()
                .WithTitle("Sorry, I do not support this environment!")
                .WithDescription("Please use a channel in a server.\n" +
                                 "If you dont have access to a server with me join the official scrap discord:\n" +
                                 "https://discord.gg/xu9NUcv")
                .WithColor(Color.Red)
                .Build();

        public static Embed ConstructionEmbed =>
            new EmbedBuilder()
             .WithTitle("Processing :man_construction_worker:")
             .WithDescription("Please wait for me to collect the data :mag:")
             .WithColor(Color.Purple)
             .Build();

        public static Embed NotFoundEmbed(string entityType, string entityName)
            => new EmbedBuilder()
                .WithColor(Color.Red)
                .WithTitle($"Could not find the {entityType} '{entityName}'! :rotating_light:")
                .WithDescription(":pen_ballpoint:Check you spelling!")
                .Build();

        public static Embed TeamEmbed(Team team, Member leader, int seasonWins)
            => new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle($"Statistics of {team.Name}")
                .WithDescription($"**Description**:\n{team.Description}")
                .AddField("Trophies :trophy:", team.Trophies)
                .AddField("Members :raising_hand:", team.PlayerCount)
                .AddField("Required Trophies :pray:", team.RequiredTrophies)
                .AddField("Season Wins :muscle:", seasonWins)
                .AddField("Leader :crown:", $"{leader.Name} ({leader.Trophies} :trophy:)")
                .Build();

        public static Embed PlayerEmbed(Player player, Team team, Member member)
        {
            var playerEmbed = new EmbedBuilder()
                .WithColor(Color.Green)
                .WithTitle($"Statistics of {player.Name}")
                .AddField("Level <:crown:444526211574792192>", player.Level, true)
                .AddField("Trophies :trophy:", player.Trophies, true)
                .AddField("Matches Won :muscle:", player.MatchesWon, true)
                .AddField("Trophy Record :arrow_double_up:", player.MaxTrophies, true);

            if (team is not null)
            {
                playerEmbed
                    .AddField("Team :family_mwbb:", $"{team.Name} ({team.Trophies} :trophy:)", true)
                    .AddField("Wins This Season :zap:", member.SeasonWins, true)
                    .AddField("Active <:tire:619850577475665931>", member.IsActive ? "Nope :(" : "Sure :D", true);
            }
            else
            {
                playerEmbed.AddField("Team :man_gesturing_no:", "---", true);
            }

            return playerEmbed.Build();
        }

        public static Embed FailedResultEmbed(FailedResult result)
        {
            var description = new StringBuilder();

            switch (result)
            {
                case ChecksFailedResult checksResult:
                    description.AppendLine("__The following check(s) failed:__");
                    foreach (var check in checksResult.FailedChecks)
                    {
                        description.AppendLine($"- `{check.Result.FailureReason}`");
                    }
                    break;
                case ParameterChecksFailedResult checksResult:
                    description.AppendLine("__The following check(s) failed:__");
                    foreach (var check in checksResult.FailedChecks)
                    {
                        description.AppendLine($"- `{check.Result.FailureReason}`");
                    }
                    break;
                case ArgumentParseFailedResult argResult:
                    description.AppendLine("__**The following parameters are missing:**__");
                    description.AppendLine($"`{argResult.FailureReason}`");
                    break;
                case OverloadsFailedResult overloadsResult:
                    description.AppendLine("__**The syntax was wrong**__");
                    description.AppendLine($"`{overloadsResult.FailureReason}`");
                    description.AppendLine($"`{overloadsResult.FailedOverloads.First().Value.FailureReason}`");
                    break;
                case TypeParseFailedResult parseResult:
                    description.AppendLine("__**The following parameter couldn't be parsed**__");
                    description.AppendLine($"`[{parseResult.Parameter.Name}] - {parseResult.FailureReason}`");
                    break;
            }

            return new EmbedBuilder()
             .WithColor(Color.Red)
             .WithTitle("**Something went wrong!**")
             .WithDescription(description.ToString())
             .Build();
        }
    }
}

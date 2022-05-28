﻿using Discord;
using Qmmands;
using ScrapBot.Commands;
using ScrapBot.Utils;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrapBot.Extensions
{
    public static partial class Extensions
    {
        public static string GetUsageMessage(this Command command)
        {
            var builder = new StringBuilder()
                .Append($"**{command.Name}**");

            foreach (var parameter in command.Parameters)
            {
                builder.Append($" [{parameter.Name}]");
            }

            builder.Append($" - {command.Description}");

            return builder.ToString();
        }

        public static Embed GetHelpEmbed(this Command command)
        {
            var builder = new EmbedBuilder()
                .WithColor(EmbedColor.Help)
                .WithTitle($"{command.Name} Overview")
                .AddField("Usage", GetUsageMessage(command))
                .AddField("Aliases", $"{System.String.Join(", ", command.Aliases)}")
                .AddField("Description", command.Description ?? "None");

            if (!System.String.IsNullOrWhiteSpace(command.Remarks))
            {
                builder.AddField("Remarks", command.Remarks);
            }


            if (command.Checks.Count > 0)
            {
                var checkDescriptions = command.Checks.Select(x => x as ScrapCheckAttribute)
                                                      .Select(x => x.Description);

                builder.AddField("Checks", System.String.Join("- ", checkDescriptions));

            }

            return builder.Build();
        }

        public static async Task<bool> RunHardChecksAsync(this Command command, ScrapContext context)
        {
            var checks = command.Checks
                .Where(x => x is HardCheckAttribute)
                .Select(x => x as HardCheckAttribute).ToList();

            var checkResults = await Task.WhenAll(checks.Select(x => x.CheckAsync(context).AsTask()));

            return !checkResults.Any(x => !x.IsSuccessful);
        }

        public static async Task<bool> RunSoftChecksAsync(this Command command, ScrapContext context)
        {
            var checks = command.Checks
                .Where(x => x is SoftCheckAttribute)
                .Select(x => x as SoftCheckAttribute).ToList();

            var checkResults = await Task.WhenAll(checks.Select(x => x.CheckAsync(context).AsTask()));

            return checkResults.Any(x => !x.IsSuccessful);
        }
    }
}

using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Qmmands;
using ScrapBot.Commands;
using ScrapBot.Utils;

namespace ScrapBot.Services
{
    public sealed class CommandHandlerService : ScrapService
    {
#pragma warning disable
        [InjectAttribute] private readonly DiscordShardedClient client;
        [InjectAttribute] private readonly CommandService commands;
        [InjectAttribute] private readonly IConfiguration config;
        [InjectAttribute] private readonly LoggerService logger;
        [InjectAttribute] private readonly IServiceProvider provider;
#pragma warning restore

        public override Task InitializeAsync()
        {
            client.MessageReceived += HandleMessageAsync;
            commands.CommandExecuted += CommandExecutedAsync;
            commands.CommandExecutionFailed += CommandExecutionFailedAsync;
            return base.InitializeAsync();
        }

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (!CommandUtilities.HasPrefix(message.Content, config["prefix"], StringComparison.OrdinalIgnoreCase, out string output))
            {
                return;
            }
            if (!(message.Channel is SocketTextChannel textChannel) || !(message.Author is SocketGuildUser guildUser))
            {
                await message.Channel.SendMessageAsync(embed: EmbedUtils.UnsupportedEnvironment);
                return;
            }
            if (!guildUser.Guild.CurrentUser.GetPermissions(textChannel).SendMessages)
            {
                return;
            }

            var context = new ScrapContext(client, message as SocketUserMessage, provider);
            var result = await commands.ExecuteAsync(output, context);

            if (!(result is FailedResult failedResult))
            {
                return;
            }
            if (failedResult is CommandNotFoundResult)
            {
                return;
            }

            await message.Channel.SendMessageAsync(embed: EmbedUtils.FailedResultEmbed(failedResult));
        }

        private async Task CommandExecutedAsync(CommandExecutedEventArgs args)
        {
            var message = new LogMessage(LogSeverity.Info, "CmdHandler", "Command Executed Successfully!");
            await logger.LogAsync(message);
        }

        private async Task CommandExecutionFailedAsync(CommandExecutionFailedEventArgs args)
        {
            var ctx = args.Context as ScrapContext;

            if (args.Result.Exception != null)
            {
                await logger.ReportErrorAsync(ctx.Message, args.Result.Exception);
            }

            var message = new LogMessage(LogSeverity.Warning, "CmdHandler", "An error occured while executing a command!", args.Result.Exception);
            await logger.LogAsync(message);
        }
    }
}
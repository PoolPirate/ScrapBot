using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Qmmands;
using ScrapBot.Commands;
using ScrapBot.Utils;
using System;
using System.Threading.Tasks;

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

        private ValueTask Commands_CommandExecuted(object sender, CommandExecutedEventArgs e) => throw new NotImplementedException();

        private async Task HandleMessageAsync(SocketMessage message)
        {
            if (!CommandUtilities.HasPrefix(message.Content, config["prefix"], StringComparison.OrdinalIgnoreCase, out string output))
            {
                return;
            }
            if (message.Channel is not SocketTextChannel textChannel || message.Author is not SocketGuildUser guildUser)
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

            if (result is not FailedResult failedResult)
            {
                return;
            }
            if (failedResult is CommandNotFoundResult)
            {
                return;
            }

            await message.Channel.SendMessageAsync(embed: EmbedUtils.FailedResultEmbed(failedResult));
        }

        private async ValueTask CommandExecutedAsync(object sender, CommandExecutedEventArgs e)
        {
            var message = new LogMessage(LogSeverity.Info, "CmdHandler", "Command Executed Successfully!");
            await logger.LogAsync(message);
        }

        private async ValueTask CommandExecutionFailedAsync(object sender, CommandExecutionFailedEventArgs e)
        {
            var ctx = e.Context as ScrapContext;

            if (e.Result.Exception is not null)
            {
                await logger.ReportErrorAsync(ctx.Message, e.Result.Exception);
            }

            var message = new LogMessage(LogSeverity.Warning, "CmdHandler", "An error occured while executing a command!", e.Result.Exception);
            await logger.LogAsync(message);
        }
    }
}
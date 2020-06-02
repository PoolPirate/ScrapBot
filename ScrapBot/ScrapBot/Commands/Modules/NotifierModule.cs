using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Interactivity;
using Interactivity.Confirmation;
using Interactivity.Selection;
using Qmmands;
using ScrapBot.Entities;
using ScrapBot.Utils;

namespace ScrapBot.Commands
{
    [Name("Notifier <:ping:633400465106796546>")]
    [RequireUser(712755685863194755, Group = "Access"), RequireOwner(Group = "Access")]
    public class NotifierModule : ScrapModule
    {
        public ScrapDbContext DbContext { get; set; }
        public InteractivityService Interactivity { get; set; }

        [Command("Notify", "N")]
        [Description("Create a notifier")]
        [RequireBotPermission(ChannelPermission.ManageMessages | ChannelPermission.AddReactions)]
        public async Task CreateNotifierAsync()
        {
            var typeSelection = new MessageSelectionBuilder<NotifierType>()
                .WithValues(Enum.GetValues(typeof(NotifierType)).Cast<NotifierType>())
                .WithUsers(Context.User)
                .WithAllowCancel(true)
                .WithDeletion(DeletionOptions.AfterCapturedContext | DeletionOptions.Valid)
                .Build();

            var typeResult = await Interactivity.SendSelectionAsync(typeSelection, Context.Channel);

            if (!typeResult.IsSuccess)
            {
                return;
            }

            var message = await Context.Channel.SendMessageAsync(embed: new EmbedBuilder()
                .WithColor(EmbedColor.Info)
                .WithTitle("How many minutes should I wait before notifying you again?")
                .Build());

            var intervalResult = await Interactivity.NextMessageAsync(message =>
            {
                return message.Channel == Context.Channel &&
                    message.Author == Context.User &&
                    int.TryParse(message.Content, out int minutes) &&
                    minutes > 0;
            });

            if (!intervalResult.IsSuccess)
            {
                return;
            }

            await message.DeleteAsync();
            await intervalResult.Value.DeleteAsync();

            var interval = TimeSpan.FromMinutes(int.Parse(intervalResult.Value.Content));

            //var compactConfirmation = new ConfirmationBuilder()
            //    .WithDeletion(DeletionOptions.AfterCapturedContext)
            //    .WithUsers(Context.User)
            //    .WithContent(PageUtils.CompactDisplay)
            //    .Build();

            //var compactResult = await Interactivity.SendConfirmationAsync(compactConfirmation, Context.Channel);

            //if (!compactResult.IsSuccess)
            //{
            //    return;
            //}

            await ReplyAsync("Done!");

            var notifier = new Notifier(Context.User.Id, /*compactResult.Value*/ false, typeResult.Value, interval);

            DbContext.Notifiers.Add(notifier);
            await DbContext.SaveChangesAsync();
        }

    }
}

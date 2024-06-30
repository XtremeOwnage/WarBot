namespace WarBot.Modules.GuildCommand;

[RequireContext(ContextType.Guild)]
public class ClearMessagesModule : WarBOTModule
{
    [MessageCommand("Clear Messages After")]
    [RequireBotPermission(Discord.ChannelPermission.ManageMessages)]
    public async Task ClearMessagesAfter(IMessage message)
    {
        await DeferAsync();

        var toDelete = await message.Channel.GetMessagesAsync(message.Id, Direction.After, 1000).FlattenAsync();
        await (message.Channel as SocketTextChannel).DeleteMessagesAsync(toDelete);

        await Context.Interaction.ModifyOriginalResponseAsync(o =>
        {
            o.Content = "Done.";
            o.Flags = new Optional<MessageFlags?>(MessageFlags.Ephemeral);
        });
    }


    [SlashCommand("clear", "Delete specified messages from a channel. Will exclude pinned messages unless specified.")]
    [RoleLevel(RoleLevel.Leader)]
    [RequireBotPermission(ChannelPermission.ViewChannel, NotAGuildErrorMessage = "This command only works inside of a discord guild.")]
    [RequireBotPermission(ChannelPermission.ManageMessages, NotAGuildErrorMessage = "This command only works inside of a discord guild.")]
    ///action
    ///NonPinned = Only non-pinned messages
    ///Pinned = Only Pinned Messages
    ///ALL = Everything!
    public async Task ClearMessages([Choice("Pinned", "pinned"), Choice("Extended", "extended")] string action = "")
    {
        if (Context.Channel is SocketTextChannel stc)
        {
            await RespondAsync("Please wait...", ephemeral: true);
            bool SelectPinned = action.ToLowerInvariant().Contains("-pinned");
            bool Extended = action.ToLowerInvariant().Contains("-extended");

            DateTimeOffset discordBulkCutoffDate = DateTimeOffset.Now.AddDays(-14);

            //Bulk Delete messages
            while (true)
            {
                IAsyncEnumerable<IReadOnlyCollection<IMessage>> asyncresults = stc.GetMessagesAsync(500);
                IEnumerable<IMessage> results = await asyncresults.FlattenAsync();

                List<IMessage> ToBulkDelete = results
                    .Where(o => o.CreatedAt > discordBulkCutoffDate)
                    .ToList();

                if (SelectPinned == false)
                    ToBulkDelete = ToBulkDelete
                        .Where(o => o.IsPinned == false)
                        .ToList();


                //If there are messages to bulk delete, do it.
                if (ToBulkDelete.Count > 0)
                    await stc.DeleteMessagesAsync(ToBulkDelete);
                else
                    break;

            }

            //If an extended delete was not requested, return now.
            if (!Extended)
            {
                await Context.Interaction.ModifyOriginalResponseAsync(o =>
                {
                    o.Content = "Done.";
                });
                return;
            }
            await RespondAsync("My ability to delete messages past the bulk delete peroid is currently not yet re-implemented.", ephemeral: true);
            //#region Long Term, Bulk Delete.
            //RequestOptions options = RequestOptions.Default;
            //options.Timeout = (int)TimeSpan.FromMinutes(5).TotalSeconds;
            //options.RetryMode = RetryMode.RetryRatelimit;

            //ITextChannel ch = Channel as ITextChannel;
            //int FailCount = 0;
            //while (true)
            //{
            //    try
            //    {

            //        List<IMessage> msgs = ch
            //            .GetMessagesAsync(1, CacheMode.AllowDownload, options)
            //            .FlattenAsync()?
            //            .Result?
            //            .ToList();

            //        if (msgs == null || msgs.Count == 0)
            //            break;

            //        if (DeletePinned == false)
            //            msgs = msgs
            //                .Where(o => o.IsPinned == false)
            //                .ToList();

            //        foreach (IMessage msg in msgs)
            //        {
            //            await msg.DeleteAsync(options);

            //            await Task.Delay(50);
            //        }
            //    }
            //    catch (System.TimeoutException)
            //    {
            //        FailCount++;

            //        if (FailCount == 20)
            //        {
            //            await Channel.SendMessageAsync("Encountered too many errors. Please run the command again later.");
            //            throw;
            //        }

            //        //await Log.ConsoleOUT("TaskBot - Bulk delete sleeping for 1 minute. Caught timeout");

            //        await Channel.SendMessageAsync("-Sleeping for 1 minute, due to Discord's API policies.-");
            //        //Wait 10 seconds.
            //        await Task.Delay((int)TimeSpan.FromMinutes(1).TotalMilliseconds);
            //    }
            //}
            //#endregion
        }
        else
            await RespondAsync("I cannot clear messages from this channel.");
    }
}
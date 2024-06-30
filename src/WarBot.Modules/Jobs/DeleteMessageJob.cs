using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.Jobs;
public class DeleteMessageJob
{
    private readonly IDiscordClient client;
    private readonly ILogger<DeleteMessageJob> log;

    public DeleteMessageJob(IDiscordClient client, ILogger<DeleteMessageJob> LOG)
    {
        this.client = client;
        log = LOG;
    }

    /// <summary>
    /// Enqueue a background job to delete the specified <paramref name="Message"/> after <paramref name="Minutes"/> minutes.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="FinalEvent">Notates if this is the final event. If this is the final event, AND the specified logic is set to CLEAR ENTIRE CHANNEL, job will be enqueued to nuke channel.</param>
    public static void Enqueue(IMessage Message, WarBot.DataAccess.Logic.Events.GuildChannelEventLogic Logic)
    {
        var delay = TimeSpan.FromMinutes(Logic.ClearDurationMins);
        var guild = Logic.GuildLogic.guild.Id;
        var ch = Message.Channel.Id;

        Enqueue(Logic.ClearMethod, delay, guild, ch, Message.Id, true);
    }

    // <summary>
    /// Enqueue a background job to delete the specified <paramref name="Message"/> after <paramref name="Minutes"/> minutes.
    /// </summary>
    /// <param name="Message"></param>
    /// <param name="FinalEvent">Notates if this is the final event. If this is the final event, AND the specified logic is set to CLEAR ENTIRE CHANNEL, job will be enqueued to nuke channel.</param>
    public static void Enqueue(IMessage Message, WarBot.DataAccess.Logic.Events.HustleGuildChannelEventLogic Logic, bool FinalEvent = false)
    {
        var delay = TimeSpan.FromMinutes(Logic.ClearDurationMins);
        var guild = Logic.GuildLogic.guild.Id;
        var ch = Message.Channel.Id;

        Enqueue(Logic.ClearMethod, delay, guild, ch, Message.Id, FinalEvent);
    }

    private static void Enqueue(EventClearType ClearMethod, TimeSpan Delay, ulong guild, ulong channel, ulong message, bool FinalEvent)
    {
        if (ClearMethod == DataAccess.Logic.Events.EventClearType.DISABLED)
            return;

        if (ClearMethod == DataAccess.Logic.Events.EventClearType.INDIVIDUAL_MESSAGE_TIMER)
        {
            Hangfire.BackgroundJob.Schedule<DeleteMessageJob>(o => o.DeleteMessageAsync(guild, channel, message), Delay);
            return;
        }
        else if (ClearMethod == DataAccess.Logic.Events.EventClearType.ENTIRE_CHANNEL && FinalEvent == true)
        {
            Hangfire.BackgroundJob.Schedule<DeleteMessageJob>(o => o.ClearChannelAsync(guild, channel), Delay);
            return;
        }
    }

    public async Task ClearChannelAsync(ulong GuildID, ulong ChannelID)
    {
        var guild = await client.GetGuildAsync(GuildID);
        if (guild is null)
        {
            log.LogError("Error, Could not find guild?");
            return;
        }

        var Channel = await guild.GetChannelAsync(ChannelID);

        if (Channel is null)
        {
            log.LogError("Error- Could not find channel");
            return;
        }
        try
        {

            if (Channel is SocketTextChannel stc)
            {
                DateTimeOffset discordBulkCutoffDate = DateTimeOffset.Now.AddDays(-14);

                //Bulk Delete messages
                while (true)
                {
                    IAsyncEnumerable<IReadOnlyCollection<IMessage>> asyncresults = stc.GetMessagesAsync(500);
                    IEnumerable<IMessage> results = await asyncresults.FlattenAsync();

                    List<IMessage> ToBulkDelete = results
                        .Where(o => o.CreatedAt > discordBulkCutoffDate)
                        .ToList();

                    //If there are messages to bulk delete, do it.
                    if (ToBulkDelete.Count > 0)
                        await stc.DeleteMessagesAsync(ToBulkDelete);
                    else
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            log.LogError($"Failed to clear channel {Channel?.Name ?? "NULL"}.");
            log.LogError(ex.ToString());
        }
    }

    public async Task DeleteMessageAsync(ulong GuildID, ulong ChannelID, ulong MessageID)
    {
        var Guild = await client.GetGuildAsync(GuildID);
        if (Guild is null)
        {
            log.LogError("Could not find Guild by ID.");
            return;
        }
        var Channel = await Guild.GetChannelAsync(ChannelID);
        try
        {
            if (Channel is null)
            {
                log.LogError("Error- Could not find channel");
                return;
            }

            var textchan = Channel as ITextChannel;

            if (textchan is null)
            {
                log.LogError("Channel is not ITextChannel?!?");
                return;
            }

            var message = await textchan.GetMessageAsync(MessageID);
            if (message is not null)
            {
                await message.DeleteAsync();
                return;
            }

            log.LogInformation("Message was not found. Trying.... another approach");

            var messages = await textchan.GetMessagesAsync(100).FlattenAsync();

            var matching = messages.FirstOrDefault(o => o.Id == MessageID);

            if (matching is not null)
            {
                log.LogInformation("Found matching message!");
                await matching.DeleteAsync();
                return;
            }

            log.LogWarning("Could not find any matching messages.... Oh well?");
            return;

        }
        catch (Exception ex)
        {
            log.LogError($"Failed to delete message {MessageID} in channel {Channel?.Name ?? "NULL"}");
            log.LogError(ex.ToString());
        }
    }
}


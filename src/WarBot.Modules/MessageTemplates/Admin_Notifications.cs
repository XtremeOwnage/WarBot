using WarBot.Data;
using WarBot.Modules.Jobs;

namespace WarBot.Modules.MessageTemplates;

/// <summary>
/// This class is used to relay important messages to a guild's leadership or owner.
/// </summary>
public class Admin_Notifications
{
    private readonly IDiscordClient client;
    private readonly ILogger<Admin_Notifications> log;
    private readonly WarDB db;

    public Admin_Notifications(IDiscordClient client, ILogger<Admin_Notifications> log, WarDB db)
    {
        this.client = client;
        this.log = log;
        this.db = db;
    }

    public async Task SendMessage(ulong Guild, string Message)
    {
        var guild = await client.GetGuildAsync(Guild);
        if (guild is null)
        {
            log.LogWarning("Unable to find guild by ID.");
            return;
        }

        using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);

        //First, send the message to the admins channel.
        var adminChannel = await logic.Channel_Admins.GetChannelAsync();
        if (adminChannel is not null)
        {
            await adminChannel.SendMessageAsync(Message);
            log.LogDebug($"Delivered message to guild {guild.Name} via Admin Channel");
            return;
        }

        //Next, try the updates channel.

        var updatesChannel = logic.Event_Updates.Enabled
            ? await logic.Event_Updates.Channel.GetChannelAsync()
            : null;

        if (updatesChannel is not null && await updatesChannel.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages))
        {
            await updatesChannel.SendMessageAsync(Message);
            log.LogDebug($"Delivered message to guild {guild.Name} via Updates Channel");
            return;
        }

        var defaultChannel = await guild.GetDefaultChannelAsync();
        if (defaultChannel is not null && await defaultChannel.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages))
        {
            await defaultChannel.SendMessageAsync(Message);
            log.LogDebug($"Delivered message to guild {guild.Name} via Default Channel");
            return;
        }

        //Lastly, send a DM to the owner.
        var owner = await guild.GetOwnerAsync();
        if (owner is not null)
        {
            await owner.SendMessageAsync(Message);
            log.LogDebug($"Delivered message to guild {guild.Name} via Owner DM");
        }

        log.LogDebug($"Could not deliver message to guild {guild.Name}.");
    }

    /// <summary>
    /// Will attempt to send the specified message either via the guilds' configured updates channel, OR by direct PM to the guild's owner.
    /// </summary>
    public async Task SendUpdate(ulong Guild, string Message)
    {
        var guild = await client.GetGuildAsync(Guild);
        if (guild is null)
        {
            log.LogWarning("Unable to find guild by ID.");
            return;
        }

        using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);

        var updatesChannel = logic.Event_Updates.Enabled
            ? await logic.Event_Updates.Channel.GetChannelAsync()
            : null;

        if (updatesChannel is not null && await updatesChannel.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages))
        {
            var msg = await updatesChannel.SendMessageAsync(Message);

            log.LogDebug($"Message for guild {guild.Name} delivered via updates channel");

            DeleteMessageJob.Enqueue(msg, logic.Event_Updates);
            return;
        }

        try
        {
            //Lastly, send a DM to the owner.
            var owner = await guild.GetOwnerAsync();
            if (owner is null)
            {
                log.LogInformation($"Could not get owner for guild {guild.Name}. Message will not be delivered.");
            }
            else
            {
                await owner.SendMessageAsync(Message);
                log.LogDebug($"Message for guild {guild.Name} delivered via DM to owner.");
                return;
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Failed to deliver update notification for guild " + guild.Name);
        }
    }
    //public async Task SendEmbed(ulong Guild, Embed Embed)
    //{
    //    var guild = await client.GetGuildAsync(Guild);
    //    if (guild is null)
    //    {
    //        log.LogWarning("Unable to find guild by ID.");
    //        return;
    //    }

    //    using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);

    //    //First, send the message to the admins channel.
    //    var adminChannel = await logic.Channel_Admins.GetChannelAsync();
    //    if (adminChannel is not null)
    //    {
    //        await adminChannel.SendMessageAsync(embed: Embed);
    //        return;
    //    }

    //    //Next, try the updates channel.

    //    var updatesChannel = logic.Event_Updates.Enabled
    //        ? await logic.Event_Updates.Channel.GetChannelAsync()
    //        : null;

    //    if (updatesChannel is not null)
    //    {
    //        await updatesChannel.SendMessageAsync(embed: Embed);
    //        return;
    //    }

    //    //Lastly, send a DM to the owner.
    //    var owner = await guild.GetOwnerAsync();
    //    await owner.SendMessageAsync(embed: Embed);
    //}


}

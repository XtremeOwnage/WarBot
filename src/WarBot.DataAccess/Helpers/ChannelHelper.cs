using Discord;

namespace WarBot.DataAccess.Helpers;
public class ChannelHelper
{
    /// <summary>
    /// Finds the first TextChannel we can send messages to. Will first try the default channel.
    /// If we are unable to find a channel which we can send messages to, we will create a new channel, if we have permissions.
    /// </summary>
    /// <returns></returns>
    public static async Task<ITextChannel?> FindChannel_For_Welcome_Message(IGuild Guild)
    {
        var myUser = await Guild.GetCurrentUserAsync();
        var defaultGuildChannel = await Guild.GetDefaultChannelAsync();


        //First, test the default channel.
        if (defaultGuildChannel != null && myUser.GetPermissions(defaultGuildChannel).SendMessages)
            return defaultGuildChannel;

        await Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to send to default channel.");

        //Next, Just loop through channels, until we find a writeable channel.
        var allGuildChannels = await Guild.GetChannelsAsync();
        var ch = allGuildChannels.OfType<ITextChannel>().FirstOrDefault(o => myUser.GetPermissions(o).SendMessages == true);
        if (ch != null)
            return ch;

        await Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to send to ANY channel.");

        //Try to make a DM with the guild's owner.
        try
        {
            var guildOwner = await Guild.GetOwnerAsync();
            var dm = await guildOwner.CreateDMChannelAsync();
            await dm.SendMessageAsync($"I am unable to send the following message to your discord guild {Guild.Name} because, I lack the SEND_MESSAGES permission for all channels.\r" +
                $"\nTo note: I will not be very useful if I cannot send messages to your guild.\r" +
                $"\nPlease grant me the permissions to send messages in a guild channel, and type 'bot, setup' to configure me.");
        }
        catch
        {
            await Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to DM Owner");
        }

        return null;
    }

    /// <summary>
    /// Finds the first channel, which is private to everybody, but, viewable by admins.
    /// </summary>
    /// <param name="Guild"></param>
    /// 
    /// <returns></returns>
    public static async Task<ITextChannel?> findFirstAdminChannel(IGuild Guild)
    {
        var ME = await Guild.GetCurrentUserAsync();

        var AllGuildChannels = await Guild.GetChannelsAsync();
        //Next, Just loop through channels, until we find a writeable channel.
        foreach (ITextChannel tch in AllGuildChannels.OfType<ITextChannel>())
            //Simple stupid way. //ToDo - Add better logic in the future.
            //Summary - Find a channel we can write to, but, everybody cannot.
            if (!tch.TestPermission(ChannelPermission.ViewChannel, Guild.EveryoneRole)
                && ME.GetPermissions(tch).SendMessages)
                return tch;

        //If, there are no writable channels. Lets create one?
        if (ME.GuildPermissions.ManageChannels && ME.GuildPermissions.SendMessages)
        {
            var newCh = await Guild.CreateTextChannelAsync("Admins");

            //Block EVERYONE role from all permissions related to this channel.
            await newCh.AddPermissionOverwriteAsync(Guild.EveryoneRole, OverwritePermissions.DenyAll(newCh));

            //Grant the highest role, all permissions.
            await newCh.AddPermissionOverwriteAsync(Guild.Roles.First(), OverwritePermissions.AllowAll(newCh));

            await newCh.SendMessageAsync(@"I was unable to find a channel to which I could sent leadership/officer messages to... So, I created this channel automatically.
I will leadership-related messages into this channel.");
            return newCh;
        }

        return null;
    }
}

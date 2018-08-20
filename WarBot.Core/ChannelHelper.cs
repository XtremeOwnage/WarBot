using Discord;
using System.Linq;
using System.Threading.Tasks;

namespace WarBot.Core
{
    public class ChannelHelper
    {
        /// <summary>
        /// Finds the first TextChannel we can send messages to. Will first try the default channel.
        /// If we are unable to find a channel which we can send messages to, we will create a new channel, if we have permissions.
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static async Task<ITextChannel> findDefaultChannel(IDiscordClient Client, IGuild Guild)
        {
            var myUser = await Guild.GetCurrentUserAsync();

            //First, test the default channel.
            var DefaultChannel = await Guild.GetDefaultChannelAsync(CacheMode.AllowDownload);
            if (DefaultChannel != null && PermissionHelper.TestPermission(DefaultChannel, ChannelPermission.SendMessages, myUser))
                return DefaultChannel;

            //Next, Just loop through channels, until we find a writeable channel.
            foreach (IGuildChannel ch in await Guild.GetChannelsAsync())
            {
                //We only care about text channels.
                if (ch is ITextChannel tch)
                {
                    if (PermissionHelper.TestPermission(tch, ChannelPermission.SendMessages, myUser))
                        return tch;
                }

            }

            //If, there are no writable channels. Lets create one?
            if (myUser.GuildPermissions.ManageChannels && myUser.GuildPermissions.SendMessages)
            {
                var newCh = await Guild.CreateTextChannelAsync("General");
                await newCh.SendMessageAsync(@"I was unable to find a channel to which I could sent messages, So, I created this channel automatically.
I will post messages into this channel.");
                return newCh;
            }

            return null;
        }

        /// <summary>
        /// Finds the first channel, which is private to everybody, but, viewable by admins.
        /// </summary>
        /// <param name="Client"></param>
        /// <param name="Guild"></param>
        /// <returns></returns>
        public static async Task<ITextChannel> findFirstAdminChannel(IDiscordClient Client, IGuild Guild)
        {
            var myUser = await Guild.GetCurrentUserAsync();

            //Next, Just loop through channels, until we find a writeable channel.
            foreach (IGuildChannel ch in await Guild.GetChannelsAsync())
            {
                //We only care about text channels.
                if (ch is ITextChannel tch)
                {
                    //Simple stupid way. //ToDo - Add better logic in the future.
                    //Summary - Find a channel we can write to, but, everybody cannot.
                    if (!PermissionHelper.TestPermission(tch, ChannelPermission.ReadMessages, Guild.EveryoneRole)
                        && PermissionHelper.TestPermission(tch, ChannelPermission.SendMessages, myUser))
                        return tch;
                }
            }

            //If, there are no writable channels. Lets create one?
            if (myUser.GuildPermissions.ManageChannels && myUser.GuildPermissions.SendMessages)
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
}

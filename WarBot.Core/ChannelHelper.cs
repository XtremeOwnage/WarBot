using Discord;
using Discord.WebSocket;
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
        /// <returns></returns>
        public static async Task<IMessageChannel> FindChannel_For_Welcome_Message(SocketGuild Guild)
        {
            var myUser = Guild.CurrentUser;

            //First, test the default channel.
            if (Guild.DefaultChannel != null && myUser.GetPermissions(Guild.DefaultChannel).SendMessages)
                return Guild.DefaultChannel;

            await System.Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to send to default channel.");

            //Next, Just loop through channels, until we find a writeable channel.
            var ch = Guild.Channels.OfType<SocketTextChannel>().FirstOrDefault(o => myUser.GetPermissions(o).SendMessages == true);
            if (ch != null)
                return ch;

            await System.Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to send to ANY channel.");

            //Try to make a DM with the guild's owner.
            try
            {
                var dm = await Guild.Owner.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync($"I am unable to send the following message to your discord guild {Guild.Name} because, I lack the SEND_MESSAGES permission for all channels.\r" +
                    $"\nTo note: I will not be very useful if I cannot send messages to your guild.\r" +
                    $"\nPlease grant me the permissions to send messages in a guild channel, and type 'bot, setup' to configure me.");

                return dm;
            }
            catch
            {
                await System.Console.Out.WriteLineAsync($"Guild {Guild.Name} - Unable to DM Owner");
                //unable to make a DM...
            }

            return null;
        }

        /// <summary>
        /// Finds the first channel, which is private to everybody, but, viewable by admins.
        /// </summary>
        /// <param name="Guild"></param>
        /// 
        /// <returns></returns>
        public static async Task<ITextChannel> findFirstAdminChannel(SocketGuild Guild)
        {
            var ME = Guild.CurrentUser;

            //Next, Just loop through channels, until we find a writeable channel.
            foreach (SocketTextChannel tch in Guild.Channels.OfType<SocketTextChannel>())
            {
                //Simple stupid way. //ToDo - Add better logic in the future.
                //Summary - Find a channel we can write to, but, everybody cannot.
                if (!PermissionHelper.TestPermission(tch, ChannelPermission.ReadMessages, Guild.EveryoneRole)
                    && ME.GetPermissions(tch).SendMessages)
                    return tch;

            }

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
}

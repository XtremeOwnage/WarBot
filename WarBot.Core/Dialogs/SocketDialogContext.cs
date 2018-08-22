using Discord.WebSocket;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WarBot.Core.Dialogs
{
    /// <summary>
    /// Used to represent a socket dialog context. Used to keep a user in a state-machine dialog context.
    /// 
    /// This dialog can take place in a DM channel, or a GuildChannel.
    /// It will keep track of the selected guild's config.
    /// </summary>
    public abstract class SocketGuildDialogContextBase
    {
        public override int GetHashCode()
        {
            //ToDo - Hash logic needs to be improved.
            return (User.Id + Channel.Id).GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(ISocketMessageChannel Channel, SocketUser User)
        => (Channel.Id + User.Id).GetHashCode();


        public bool InContext(ulong channelId, ulong userId)
        {
            return channelId == Channel.Id && User.Id == userId;
        }

        /// <summary>
        /// If the WARBot is removed from the guild, while there is an open dialog, this method will be fired.
        /// </summary>
        public async Task CloseDialog_GuildRemoved()
        {
            //If this was a guild channel context, there is nothing to do.
            if (Channel is SocketGuildChannel)
            {
                return;
            }
            else if (Channel is SocketDMChannel dm)
            {
                await dm.SendMessageAsync($"Discord guild {Config.Guild.Name} has removed me. This dialog will now be closed.");
                await dm.CloseAsync();
            }
        }

        public SocketUser User { get; }
        public SocketChannel Channel { get; }
        public IGuildConfig Config { get; }

        public abstract Task ProcessMessage(string input);
    }
}

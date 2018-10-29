using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WarBot.Core.ModuleType;

namespace WarBot.Core.Dialogs
{
    /// <summary>
    /// Used to represent a socket dialog context. Used to keep a user in a state-machine dialog context.
    /// 
    /// This dialog can take place in a DM channel, or a GuildChannel.
    /// It will keep track of the selected guild's config.
    /// </summary>
    public abstract class SocketDialogContextBase
    {
        public override int GetHashCode()
        {
            //ToDo - Hash logic needs to be improved.
            return (User.Id + Channel.Id).GetHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(ISocketMessageChannel Channel, SocketUser User)
        => (Channel.Id + User.Id).GetHashCode();

        public SocketDialogContextBase(ModuleType.CommandContext Context)
        {
            this.Bot = Context.bot;
            this.Channel = Context.Channel;
            this.User = Context.User;

            this.Key = GetHashCode(Context.Channel, Context.User);
        }
        public bool InContext(ulong channelId, ulong userId)
        {
            return channelId == Channel.Id && User.Id == userId;
        }

        public SocketUser User { get; }
        public ISocketMessageChannel Channel { get; }
        public IWARBOT Bot { get; }
        /// <summary>
        /// A list of messages which will be deleted when the dialog is closed.
        /// </summary>
        public List<IMessage> CleanupList = new List<IMessage>();
        /// <summary>
        /// A shortcut to send a message to the current channel.
        /// </summary>
        /// <param name="Message"></param>
        /// <returns></returns>
        public async Task SendAsync(string Message) => await Channel.SendMessageAsync(Message);

        /// <summary>
        /// This dialog's unique hashcode, derrived from the user/channel combination.
        /// </summary>
        public int Key { get; }


        public abstract Task ProcessMessage(SocketUserMessage input);

        /// <summary>
        /// This method is fired when the dialog is created.
        /// The default implementation does nothing. Derrived classes may override.
        /// </summary>
        /// <returns></returns>
        public virtual Task OnCreated()
        {
            return Task.CompletedTask;
        }
        /// <summary>
        /// This method is fired before the dialog is closed.
        /// The default implementation does nothing. Derrived classes may override.
        /// </summary>
        /// <returns></returns>
        public virtual async Task OnClosed()
        {

        }
    }
}

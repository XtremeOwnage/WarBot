using Discord.Commands;
using Discord.WebSocket;
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

        public SocketGuildDialogContextBase(SocketCommandContext Context)
        {
            if (Context is GuildCommandContext gcc)
            {
                this.Bot = gcc.bot;
                this.Config = gcc.cfg;
            }
            else
            {
                this.Bot = null;
                this.Config = null;
            }

            this.Channel = Context.Channel;
            this.User = Context.User;
        }
        public bool InContext(ulong channelId, ulong userId)
        {
            return channelId == Channel.Id && User.Id == userId;
        }

        public SocketUser User { get; }
        public ISocketMessageChannel Channel { get; }
        public IGuildConfig Config { get; }
        public IWARBOT Bot { get; }

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
        public virtual Task OnClosed()
        {
            return Task.CompletedTask;
        }
    }
}

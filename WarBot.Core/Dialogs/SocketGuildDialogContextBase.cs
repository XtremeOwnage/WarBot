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
    public abstract class SocketGuildDialogContextBase : SocketDialogContextBase
    {
        public SocketGuildDialogContextBase(GuildCommandContext Context)
            : base(Context)
        {
            this.Config = Context.cfg;
        }

        public IGuildConfig Config { get; }


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
    }
}

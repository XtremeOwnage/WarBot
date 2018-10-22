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
            this.Guild = Context.Guild;
            this.Channel = Context.GuildChannel;
            this.User = Context.GuildUser;
        }

        public IGuildConfig Config { get; }
        public SocketGuild Guild { get; }
        public new SocketTextChannel Channel { get; }
        public new SocketGuildUser User { get; }


        /// <summary>
        /// If the WARBot is removed from the guild, while there is an open dialog, this method will be fired.
        /// </summary>
        public Task CloseDialog_GuildRemoved()
        {
            //Do nothing.... for now.
            return Task.CompletedTask;
        }
    }
}

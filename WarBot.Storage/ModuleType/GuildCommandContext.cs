using Discord.Commands;
using Discord.WebSocket;
using WarBot.Core;
namespace WarBot.Core.ModuleType
{
    /// <summary>
    /// Contains a reference to both WarBOT, as well as a IGuildConfig.
    /// </summary>
    public class GuildCommandContext : CommandContext
    {
        //The purpose of this command context type, is to greatly reduce the dependancy on the simple DI solution to pass around references.
        //Most guild-specific modules will need a reference to the IGuildConfig, and/or the actual IWarBot.
        //By passing the references with the context, we don't have to retreive them later reducing the amount of required calls and tasks.

        public IGuildConfig cfg { get; private set; }
        public SocketGuildUser GuildUser => this.User as SocketGuildUser;
        public SocketTextChannel GuildChannel => this.Channel as SocketTextChannel;

        public GuildCommandContext(DiscordSocketClient Client, SocketUserMessage Message, IGuildConfig Config, IWARBOT WarBot)
            : base(Client, Message, WarBot)
        {
            this.cfg = Config;
        }
    }
}

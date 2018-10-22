using Discord.Commands;
using Discord.WebSocket;
namespace WarBot.Core.ModuleType
{
    /// <summary>
    /// Contains a reference to the IWarBOT.
    /// </summary>
    public class CommandContext : SocketCommandContext
    {
        //The purpose of this command context type, is to greatly reduce the dependancy on the simple DI solution to pass around references.
        //Most guild-specific modules will need a reference to the IGuildConfig, and/or the actual IWarBot.
        //By passing the references with the context, we don't have to retreive them later reducing the amount of required calls and tasks.

        public IWARBOT bot { get; private set; }
        public CommandContext(DiscordSocketClient Client, SocketUserMessage Message, IWARBOT WarBot)
            : base(Client, Message)
        {
            this.bot = WarBot;
        }
    }
}

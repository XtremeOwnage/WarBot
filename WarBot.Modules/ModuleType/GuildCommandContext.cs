using Discord.Commands;
using Discord.WebSocket;
using WarBot.Core;
namespace WarBot.Modules
{
    public class GuildCommandContext : SocketCommandContext
    {
        public IGuildConfig cfg { get; private set; }
        public GuildCommandContext(DiscordSocketClient Client, SocketUserMessage Message, IGuildConfig Config)
            : base(Client, Message)
        {
            this.cfg = Config;
        }
    }
}

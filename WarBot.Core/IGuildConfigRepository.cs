using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace WarBot.Core
{
    public interface IGuildConfigRepository
    {
        Task<IGuildConfig> GetConfig(SocketGuild Guild);
    }
}
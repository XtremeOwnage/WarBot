using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace WarBot.Core
{
    public interface IGuildConfigRepository
    {
        Task<IGuildConfig> GetConfig(SocketGuild Guild);

        /// <summary>
        /// Force a config to load as the specified environment.
        /// This should almost never be used.
        /// </summary>
        /// <param name="Guild"></param>
        /// <param name="Environment"></param>
        /// <returns></returns>
        Task<IGuildConfig> GetConfig(SocketGuild Guild, Environment? Environment);
    }
}
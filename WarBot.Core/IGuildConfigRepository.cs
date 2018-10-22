using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace WarBot.Core
{
    public interface IGuildConfigRepository
    {
        /// <summary>
        /// Force a config to load as the specified environment.
        /// This should almost never be used.
        /// </summary>
        /// <param name="Guild"></param>
        /// <returns></returns>
        Task<IGuildConfig> GetConfig(SocketGuild Guild);

        /// <summary>
        /// Returns a list of all active, cached configs.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IGuildConfig> GetCachedConfigs();

        /// <summary>
        /// Removes a guild from the internal cache.
        /// </summary>
        /// <param name="Guild"></param>
        void removeGuild(SocketGuild Guild);
    }
}
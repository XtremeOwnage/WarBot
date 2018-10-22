using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarBot.Storage.Models.Voting;

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

        #region Polls
        /// <summary>
        /// Removes a poll from the database.
        /// </summary>
        /// <param name="MessageId">ID of the message</param>
        /// <returns></returns>
        Task RemovePoll(Poll Poll);

        /// <summary>
        /// Adds a poll to the cache and persistant storage.
        /// </summary>
        /// <param name="Poll"></param>
        /// <returns></returns>
        Task AddPoll(Poll Poll);

        /// <summary>
        /// Returns an IEnumerable of the polls being currently tracked.
        /// </summary>
        /// <returns></returns>
        IEnumerable<Poll> GetCurrentPolls();

        /// <summary>
        /// Returns if this message is associated with a poll or not.
        /// </summary>
        /// <param name="MessageId"></param>
        /// <returns></returns>
        bool IsPoll(ulong MessageId, out Poll Poll);
        #endregion
    }
}
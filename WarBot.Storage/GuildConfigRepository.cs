using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Storage
{
    /// <summary>
    /// This class is responsible for Caching, Loading, and Saving Guild Configs.
    /// It will also keep track of versions, and upgrading config versions, if required.
    /// </summary>
    public class GuildConfigRepository : IGuildConfigRepository
    {
        private ConcurrentDictionary<ulong, IGuildConfig> configStore;
        private IWARBOT bot;
        private WarDB db;

        public void Initialize(IWARBOT Bot, WarDB warDB)
        {
            this.bot = Bot;
            this.db = warDB;
            configStore = new ConcurrentDictionary<ulong, IGuildConfig>();

        }

        public IEnumerable<IGuildConfig> GetCachedConfigs() => configStore.Select(o => o.Value);

        public void removeGuild(SocketGuild Guild)
        {
            if (configStore.ContainsKey(Guild.Id))
                configStore.TryRemove(Guild.Id, out _);
        }

        public async Task<IGuildConfig> GetConfig(SocketGuild Guild)
        {
            //If no guild was passed in, return null.
            if (Guild == null)
                return null;

            //If config is already loaded, return it.
            if (configStore.ContainsKey(Guild.Id))
                return configStore[Guild.Id];

            //Look in the database.          
            if (db.Guilds.FirstOrDefault(o => o.EntityId == Guild.Id).IsNotNull(out var g))
            {

                //Inflate the object before returning it.
                g.Initialize(Guild, Save);

                //Add the object to the local cache.
                configStore.TryAdd(Guild.Id, g);

                //Return the cached object.
                return configStore[Guild.Id];
            }

            var newCfg = Storage.Models.DiscordGuild.Create(Guild);

            await newCfg.SetDefaults(Guild);

            newCfg.Initialize(Guild, Save);

            //Add the new Guild to the database.
            db.Guilds.Add(newCfg);

            //Save the new config.
            await db.SaveWithOutput();


            configStore.TryAdd(Guild.Id, newCfg);
            return configStore[Guild.Id];
        }

        private async Task Save<T>(T Cfg) where T : IGuildConfig
        {
            if (Cfg is Storage.Models.DiscordGuild guild)
            {
                await db.SaveWithOutput();
            }
            else
            {
                throw new Exception("Unknown type of IGuildConfig being saved.");
            }
        }

    }
}

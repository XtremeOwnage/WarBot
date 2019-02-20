using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.Updates;

namespace WarBot.Storage
{
    /// <summary>
    /// This class is responsible for Caching, Loading, and Saving Guild Configs.
    /// It will also keep track of versions, and upgrading config versions, if required.
    /// </summary>
    public class GuildConfigRepository : IGuildConfigRepository
    {
        public IBotUpdate CurrentUpdate { get; private set; }
        private IBotUpdate[] Updates { get; set; }
        private ConcurrentDictionary<ulong, IGuildConfig> configStore;
        private IWARBOT bot;
        private WarDB db;

        public void Initialize(IWARBOT Bot, WarDB warDB)
        {
            this.bot = Bot;
            this.db = warDB;
            configStore = new ConcurrentDictionary<ulong, IGuildConfig>();

            System.Reflection.Assembly ass = System.Reflection.Assembly.GetEntryAssembly();

            Updates = typeof(WarBot.Storage.WarDB)
                .Assembly
                .DefinedTypes
                .Where(o => o.ImplementedInterfaces.Contains(typeof(IBotUpdate)))
                .Select(o => o.Assembly.CreateInstance(o.FullName))
                .OfType<IBotUpdate>()
                .ToArray();



            CurrentUpdate = Updates.Last();
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
                g.Initialize(Guild, Save, bot);

                //Add the object to the local cache.
                configStore.TryAdd(Guild.Id, g);

                //Determine if an update is required, and update.
                await updateConfigIfRequired(g);

                //Return the cached object.
                return configStore[Guild.Id];
            }
            else //Create a new config for this guild.
            {
                var newCfg = Storage.Models.DiscordGuild.Create(Guild);

                //Set defaults for the new config.
                await newCfg.SetDefaults(Guild);

                //Set the current version.
                await updateConfigIfRequired(newCfg);

                //"Inflate" the channels, users, roles, etc.
                newCfg.Initialize(Guild, Save, bot);

                //Add the new Guild to the database.
                db.Guilds.Add(newCfg);

                //Save the new config.
                await db.SaveWithOutput();


                configStore.TryAdd(Guild.Id, newCfg);
                return configStore[Guild.Id];
            }
        }

        private async Task updateConfigIfRequired(IGuildConfig Cfg)
        {
            if (!String.IsNullOrEmpty(Cfg.BotVersion) && Double.TryParse(Cfg.BotVersion, out double CurrentVersion))
            {
                if (CurrentVersion == CurrentUpdate.Version)
                {
                    //The bot is up to date, nothing to do.
                    return;
                }
                else
                {
                    foreach (IBotUpdate update in Updates.Where(o => o.Version > CurrentVersion).OrderBy(o => o.Version))
                    {
                        //Apply the update.
                        update.Apply(Cfg);

                        //Determine if we should send an update to the guild. If so, send the update.
                        bool updateWasSent = false;

                        if (Cfg.GetGuildChannel(WarBotChannelType.WARBOT_UPDATES).IsNotNull(out var CH)
                        //Validate this update, should sent a message out.
                        && update.SendUpdateToGuild
                        //Validate the guild is opt-in to receive update messages.
                        && Cfg[Setting_Key.WARBOT_UPDATES].Enabled
                        //Validate I have permissions to send to this channel.
                        && Cfg.CurrentUser.GetPermissions(CH).SendMessages)
                        {
                            updateWasSent = true;
                            var eb = new EmbedBuilder()
                                .WithTitle($"WarBot updated to version {update.Version}")
                                .WithDescription($"I have been updated to version {update.Version} 👏")
                                .WithFooter("To view my patch notes, click this embed.")
                                .WithUrl(update.ReleaseNotesURL);

                            await CH.SendMessageAsync(embed: eb.Build());
                        }

                        await bot.Log.GuildUpdated(Cfg.Guild.Name, CurrentVersion, update.Version, updateWasSent);

                        //Update the local current version.
                        CurrentVersion = update.Version;
                    }

                    Cfg.BotVersion = CurrentUpdate.Version.ToString();
                }
            }
            else //Unable to parse the version. Assume no updates are required.
            {
                Cfg.BotVersion = CurrentUpdate.Version.ToString();
            }


            //Save the changes.
            await Cfg.SaveConfig();

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

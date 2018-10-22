using Discord;
using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Storage.Models;
using WarBot.Storage.Models.Voting;

namespace WarBot.Storage
{
    /// <summary>
    /// This class is responsible for Caching, Loading, and Saving Guild Configs.
    /// It will also keep track of versions, and upgrading config versions, if required.
    /// </summary>
    public class ConfigRepository : IGuildConfigRepository
    {
        private Dictionary<ulong, IGuildConfig> configStore;
        private Dictionary<ulong, Poll> messageToPoll;

        private IWARBOT bot;
        private WarDB db;

        public async Task Initialize(IWARBOT Bot, WarDB warDB)
        {
            this.bot = Bot;
            this.db = warDB;
            configStore = new Dictionary<ulong, IGuildConfig>();
            messageToPoll = await db.Polls.ToDictionaryAsync(o => o.MessageId, o => o);

            Console.WriteLine("Loading Polls.");
            foreach (var poll in messageToPoll)
            {
                try
                {
                    await poll.Value.LoadEntity(bot.Client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while loading poll: {ex.Message}");

                    try
                    {
                        //The poll is dead?
                        await RemovePoll(poll.Value);
                    }
                    catch (Exception ex2)
                    {
                        Console.WriteLine(ex2);
                    }
                }
            }
        }

        public IEnumerable<IGuildConfig> GetCachedConfigs()
            => configStore.Select(o => o.Value);

        public void removeGuild(SocketGuild Guild)
        {
            if (configStore.ContainsKey(Guild.Id))
                configStore.Remove(Guild.Id);
        }

        public async Task<DiscordUser> GetUser(IUser User)
        {
            var DBUser = await db.Users.FirstOrDefaultAsync(o => o.EntityId == User.Id, bot.StopToken.Token);
            if (DBUser != null)
                return DBUser;

            var U = await db.Users.AddAsync(new DiscordUser(User), bot.StopToken.Token);

            await db.SaveChangesAsync();

            return U.Entity;

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
                configStore.Add(Guild.Id, g);

                //Return the cached object.
                return configStore[Guild.Id];
            }

            //Past this point, there was no config stored in the database. Look at the old config files to determine if we can find a json file to update.

            var newCfg = Storage.Models.DiscordGuild.Create(Guild);


            var cfgTxt = await TryReadConfigFile(Guild);
            //There is a old-json based config. lets migrate it to the new database.
            if (!string.IsNullOrEmpty(cfgTxt) && (JsonConvert.DeserializeObject<Legacy.LegacyGuildConfig>(cfgTxt)).IsNotNull(out var oldCfg))
            {
                //Simple settings.
                newCfg.BotVersion = oldCfg.BotVersion;
                newCfg.WarBOT_NickName = oldCfg.NickName;
                newCfg.Website = oldCfg.WebsiteURL;
                newCfg.Loot = oldCfg.LootURL;

                //Roles
                if (oldCfg.Role_Admin?.ID != null && Guild.GetRole(oldCfg.Role_Admin.ID).IsNotNull(out var adminRole))
                    newCfg.SetGuildRole(RoleLevel.ServerAdmin, adminRole);
                if (oldCfg.Role_Leader?.ID != null && Guild.GetRole(oldCfg.Role_Leader.ID).IsNotNull(out var leaderRole))
                    newCfg.SetGuildRole(RoleLevel.Leader, leaderRole);
                if (oldCfg.Role_Officer?.ID != null && Guild.GetRole(oldCfg.Role_Officer.ID).IsNotNull(out var officerRole))
                    newCfg.SetGuildRole(RoleLevel.Officer, officerRole);
                if (oldCfg.Role_Member?.ID != null && Guild.GetRole(oldCfg.Role_Member.ID).IsNotNull(out var memberRole))
                    newCfg.SetGuildRole(RoleLevel.Member, memberRole);

                //Channels
                if (oldCfg.Channel_Officer?.ID != null && (Guild.GetChannel(oldCfg.Channel_Officer.ID) as ITextChannel).IsNotNull(out var officerChannel))
                {
                    newCfg.SetGuildChannel(WarBotChannelType.CH_Officers, officerChannel);
                    newCfg.SetGuildChannel(WarBotChannelType.CH_WarBot_Updates, officerChannel);
                }
                if (oldCfg.Channel_Member?.ID != null && (Guild.GetChannel(oldCfg.Channel_Member.ID) as ITextChannel).IsNotNull(out var memberChannel))
                {
                    newCfg.SetGuildChannel(WarBotChannelType.CH_New_Users, memberChannel);
                    newCfg.SetGuildChannel(WarBotChannelType.CH_WAR_Announcements, memberChannel);
                }


                //Migrate the notification settings over.
                newCfg.NotificationSettings.War1Enabled = oldCfg.Notifications.War1Enabled;
                newCfg.NotificationSettings.War2Enabled = oldCfg.Notifications.War2Enabled;
                newCfg.NotificationSettings.War3Enabled = oldCfg.Notifications.War3Enabled;
                newCfg.NotificationSettings.War4Enabled = oldCfg.Notifications.War4Enabled;
                newCfg.NotificationSettings.WarPrepEnding = oldCfg.Notifications.WarPrepAlmostOver;
                newCfg.NotificationSettings.WarPrepEndingMessage = oldCfg.Notifications.WarPrepEndingMessage;
                newCfg.NotificationSettings.WarPrepStarted = oldCfg.Notifications.WarPrepStarted;
                newCfg.NotificationSettings.WarPrepStartedMessage = oldCfg.Notifications.WarPrepStartedMessage;
                newCfg.NotificationSettings.WarStarted = oldCfg.Notifications.WarStarted;
                newCfg.NotificationSettings.WarStartedMessage = oldCfg.Notifications.WarStartedMessage;
                newCfg.NotificationSettings.SendUpdateMessage = oldCfg.Notifications.SendUpdateMessage;

                newCfg.Initialize(Guild, Save);

                await bot.Log.Debug($"Updated config from json to database for {Guild.Name}", Guild);
            }
            else
            {
                await newCfg.SetDefaults(Guild);

                newCfg.Initialize(Guild, Save);
            }
            //Add the new Guild to the database.
            db.Guilds.Add(newCfg);

            //Save the new config.
            await db.SaveWithOutput();


            configStore.Add(Guild.Id, newCfg);
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

        private async Task<string> TryReadConfigFile(IGuild guild)
        {
            try
            {
                var path = Path.Combine("./Guilds/", guild.Id.ToString());
                if (File.Exists(path))
                    return await File.ReadAllTextAsync(path);
                return null;
            }
            catch (Exception ex)
            {
                await bot.Log.ConsoleOUT(ex.Message);
                return null;
            }
        }

        public async Task RemovePoll(Poll Poll)
        {
            //Remove item from the local cache.
            if (messageToPoll.ContainsKey(Poll.MessageId))
                messageToPoll.Remove(Poll.MessageId);

            if (db.Polls.Contains(Poll))
            {
                db.Polls.Remove(Poll);
                //Save changes to persistant storage.
                await db.SaveChangesAsync();
            }
        }

        public async Task AddPoll(Poll Poll)
        {
            messageToPoll.TryAdd(Poll.MessageId, Poll);

            await db.Polls.AddAsync(Poll, bot.StopToken.Token);

            await db.SaveChangesAsync();
        }

        public IEnumerable<Poll> GetCurrentPolls()
         => messageToPoll.Select(o => o.Value);

        public bool IsPoll(ulong MessageId, out Poll Poll)
        {
            if (messageToPoll.ContainsKey(MessageId))
            {
                Poll = messageToPoll[MessageId];
                return true;
            }
            Poll = null;
            return false;
        }
    }
}

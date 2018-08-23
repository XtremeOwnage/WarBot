using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Storage;
using WarBot.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Discord.WebSocket;

namespace WarBot.Storage
{
    /// <summary>
    /// This class is responsible for Caching, Loading, and Saving Guild Configs.
    /// It will also keep track of versions, and upgrading config versions, if required.
    /// </summary>
    public class GuildConfigRepository : IGuildConfigRepository
    {
        private Dictionary<ulong, IGuildConfig> configStore;
        private IWARBOT bot;
        private WarDB db;

        public void Initialize(IWARBOT Bot, WarDB warDB)
        {
            this.bot = Bot;
            this.db = warDB;
            configStore = new Dictionary<ulong, IGuildConfig>();

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
            if (db.Find<Guild>(Guild.Id).IsNotNull(out var g))
            {

                //Inflate the object before returning it.
                g.Initialize(Guild, Save);

                //Add the object to the local cache.
                configStore.Add(Guild.Id, g);

                //Return the cached object.
                return configStore[Guild.Id];
            }

            //Past this point, there was no config stored in the database. Look at the old config files to determine if we can find a json file to update.

            var newCfg = Storage.Models.Guild.Create(Guild);


            var cfgTxt = await TryReadConfigFile(Guild);
            //There is a old-json based config. lets migrate it to the new database.
            if (!string.IsNullOrEmpty(cfgTxt) && (JsonConvert.DeserializeObject<Legacy.LegacyGuildConfig>(cfgTxt)).IsNotNull(out var oldCfg))
            {
                //If the old configuration's enviornment does not match the bot's current environment. Return.
                //This will prevent the TEST bot, from creating channels for PRODUCTION guilds.
                if (oldCfg.Environment != bot.Environment)
                    return null;
                //Simple settings.
                newCfg.Config.BotVersion = oldCfg.BotVersion;
                newCfg.Config.NickName = oldCfg.NickName;
                newCfg.Config.Environment = oldCfg.Environment ?? Core.Environment.PROD;
                newCfg.Config.Website = oldCfg.WebsiteURL;
                newCfg.Config.Loot = oldCfg.LootURL;

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
                newCfg.Config.NotificationSettings.War1Enabled = oldCfg.Notifications.War1Enabled;
                newCfg.Config.NotificationSettings.War2Enabled = oldCfg.Notifications.War2Enabled;
                newCfg.Config.NotificationSettings.War3Enabled = oldCfg.Notifications.War3Enabled;
                newCfg.Config.NotificationSettings.War4Enabled = oldCfg.Notifications.War4Enabled;
                newCfg.Config.NotificationSettings.WarPrepAlmostOver = oldCfg.Notifications.WarPrepAlmostOver;
                newCfg.Config.NotificationSettings.WarPrepEndingMessage = oldCfg.Notifications.WarPrepEndingMessage;
                newCfg.Config.NotificationSettings.WarPrepStarted = oldCfg.Notifications.WarPrepStarted;
                newCfg.Config.NotificationSettings.WarPrepStartedMessage = oldCfg.Notifications.WarPrepStartedMessage;
                newCfg.Config.NotificationSettings.WarStarted = oldCfg.Notifications.WarStarted;
                newCfg.Config.NotificationSettings.WarStartedMessage = oldCfg.Notifications.WarStartedMessage;
                newCfg.Config.NotificationSettings.SendUpdateMessage = oldCfg.Notifications.SendUpdateMessage;

                newCfg.Initialize(Guild, Save);

                await bot.Log.Debug($"Updated config from json to database for {Guild.Name}", Guild);
            }
            else
            {
                //If this is not the production bot, do not create a new config.
                //Reason: The set defaults method can and will create additional channels as required.
                if (this.bot.Environment != Core.Environment.PROD)
                    return null;

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
            if (Cfg is Storage.Models.Guild guild)
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
    }


}
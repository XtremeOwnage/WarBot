using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.JobScheduling;

namespace WarBot.Util
{
    public class WAR_Messages
    {
        private IWARBOT bot;
        public WAR_Messages(IWARBOT Bot)
        {
            this.bot = Bot;
        }

        /// <summary>
        /// Determine is a guild is elected into a specific war.           
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="WarNo"></param>
        /// <returns></returns>
        private bool shouldSendSpecificWar(IGuildConfig cfg, byte WarNo)
        {
            if (WarNo == 1)
                return cfg.Notifications.War1Enabled;
            else if (WarNo == 2)
                return cfg.Notifications.War2Enabled;
            else if (WarNo == 3)
                return cfg.Notifications.War3Enabled;
            else if (WarNo == 4)
                return cfg.Notifications.War4Enabled;

            else throw new ArgumentOutOfRangeException("There are only 4 wars. The value passed was not between 1 and 4.");
        }

        public async Task SendWarPrepStarted(byte WarNo)
        {
            List<Task> Tasks = new List<Task>();
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {
                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepStarted || !shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Started(cfg, ch));
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }

            Task.WaitAll(Tasks.ToArray());
        }

        public async Task SendWarPrepEnding(byte WarNo)
        {
            List<Task> Tasks = new List<Task>();
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {

                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepStarted || !shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg, ch));
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
            Task.WaitAll(Tasks.ToArray());
        }

        public async Task SendWarStarted(byte WarNo)
        {
            List<Task> Tasks = new List<Task>();
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {
                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepStarted || !shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Started(cfg, ch));
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
            Task.WaitAll(Tasks.ToArray());
        }
    }
}

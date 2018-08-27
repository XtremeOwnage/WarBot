using System;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.JobScheduling;

namespace WarBot 2.0.Util
{
    public class WAR_Messages
    {
        private IWARBOT bot;
        public WAR_Messages(IWARBOT Bot)
        {
            this.bot = Bot;
        }

        public static void ScheduleJobs(IJobScheduler job)
        {
            job.RecurringJob<WAR_Messages>("war1_prep_started", o => o.SendWarPrepStarted(1), "0 2 * * *");
            job.RecurringJob<WAR_Messages>("war2_prep_started", o => o.SendWarPrepStarted(2), "0 8 * * *");
            job.RecurringJob<WAR_Messages>("war3_prep_started", o => o.SendWarPrepStarted(3), "0 14 * * *");
            job.RecurringJob<WAR_Messages>("war4_prep_started", o => o.SendWarPrepStarted(4), "0 20 * * *");

            job.RecurringJob<WAR_Messages>("war1_prep_ending", o => o.SendWarPrepEnding(1), "45 3 * * *");
            job.RecurringJob<WAR_Messages>("war2_prep_ending", o => o.SendWarPrepEnding(2), "45 9 * * *");
            job.RecurringJob<WAR_Messages>("war3_prep_ending", o => o.SendWarPrepEnding(3), "45 15 * * *");
            job.RecurringJob<WAR_Messages>("war4_prep_ending", o => o.SendWarPrepEnding(4), "45 21 * * *");

            job.RecurringJob<WAR_Messages>("war1_started", o => o.SendWarStarted(1), "0 4 * * *");
            job.RecurringJob<WAR_Messages>("war2_started", o => o.SendWarStarted(2), "0 10 * * *");
            job.RecurringJob<WAR_Messages>("war3_started", o => o.SendWarStarted(3), "0 16 * * *");
            job.RecurringJob<WAR_Messages>("war4_started", o => o.SendWarStarted(4), "0 22 * * *");
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
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {
                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepStarted || shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    await WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Started(cfg, ch);

                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
        }

        public async Task SendWarPrepEnding(byte WarNo)
        {
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {

                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepEnding || shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    await WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg, ch);
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
        }

        public async Task SendWarStarted(byte WarNo)
        {
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {
                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarStarted || shouldSendSpecificWar(cfg, WarNo))
                        return;

                    var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements);

                    //Send the message.
                    await WarBot.Modules.MessageTemplates.WAR_Notifications.War_Started(cfg, ch);
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }

        }
    }
}

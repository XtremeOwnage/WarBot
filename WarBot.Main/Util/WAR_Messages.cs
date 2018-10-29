using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarBot.Core;

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
                    if (!cfg.Notifications.WarPrepStarted)
                        continue;
                    //Guild elected out of this specific war.
                    else if (!shouldSendSpecificWar(cfg, WarNo))
                        continue;

                    //Send the message.
                    Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Started(cfg));
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
            await Task.WhenAll(Tasks);
        }

        public async Task SendWarPrepEnding(byte WarNo)
        {
            List<Task> Tasks = new List<Task>();
            foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
            {
                try
                {
                    //Guild has elected out for this notification.
                    if (!cfg.Notifications.WarPrepEnding)
                        continue;
                    //Guild elected out of this specific war.
                    else if (!shouldSendSpecificWar(cfg, WarNo))
                        continue;

                    //Send the message.
                    Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg));
                }
                catch (Exception ex)
                {
                    await bot.Log.Error(cfg.Guild, ex);
                }
            }
            await Task.WhenAll(Tasks);
        }

        public async Task SendWarStarted(byte WarNo)
        {
            try
            {
                List<Task> Tasks = new List<Task>();
                foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
                {
                    try
                    {
                        //Guild has elected out for this notification.
                        if (!cfg.Notifications.WarStarted)
                            continue;
                        //Guild elected out of this specific war.
                        else if (!shouldSendSpecificWar(cfg, WarNo))
                            continue;

                        //Send the message.
                        Tasks.Add(WarBot.Modules.MessageTemplates.WAR_Notifications.War_Started(cfg));
                    }
                    catch (Exception ex)
                    {
                        await bot.Log.Error(cfg.Guild, ex);
                    }
                }
                await Task.WhenAll(Tasks);
            }
            catch(Exception ex2)
            {
                throw;
            }
        }

        public async Task SendPortalOpened()
        {
            try
            {
                List<Task> Tasks = new List<Task>();
                foreach (IGuildConfig cfg in bot.GuildRepo.GetCachedConfigs())
                {
                    try
                    {
                        //Guild has elected out for this notification.
                        if (!cfg.Notifications.PortalEnabled)
                            continue;

                        //Send the message.
                        Tasks.Add(WarBot.Modules.MessageTemplates.Portal_Notifications.Portal_Opened(cfg));
                    }
                    catch (Exception ex)
                    {
                        await bot.Log.Error(cfg.Guild, ex);
                    }
                }
                await Task.WhenAll(Tasks);
            }
            catch (Exception ex2)
            {
                throw;
            }
        }
    }
}

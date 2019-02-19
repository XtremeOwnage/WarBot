using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Util
{
    /// <summary>
    /// This class is called by the Job Scheduler.
    /// </summary>
    public class WAR_Messages
    {
        private IWARBOT bot;
        public WAR_Messages(IWARBOT Bot)
        {
            bot = Bot;
        }

        public async Task SendWarPrepStarted(byte WarNo)
        {
            Exception[] Errors = bot.GuildRepo
                .GetCachedConfigs()
                .Select(cfg => new Action(() =>
                {
                    //If this guild has enabled the ability to clear channel message when war is started, clear messages.
                    if (cfg[Setting_Key.CLEAR_WAR_CHANNEL_ON_WAR_START].Enabled)
                    {
                        WarBot.Modules.MessageTemplates.WAR_Notifications.clearWarChannel(cfg).Wait();
                    }

                    //Send the message.
                    WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Started(cfg, WarNo).Wait();
                })).executeParallel(bot.StopToken.Token, 1);

            foreach (Exception err in Errors)
            {
                await bot.Log.Error(null, err);
            }
        }

        public async Task SendWarPrepEnding(byte WarNo)
        {
            Exception[] Errors = bot.GuildRepo
                .GetCachedConfigs()
                .Select(cfg => new Action(() =>
                {
                    //Send the message.
                    WarBot.Modules.MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg, WarNo).Wait();
                })).executeParallel(bot.StopToken.Token, 1);

            foreach (Exception err in Errors)
            {
                await bot.Log.Error(null, err);
            }
        }

        public async Task SendWarStarted(byte WarNo)
        {
            Exception[] Errors = bot.GuildRepo
                  .GetCachedConfigs()
                  .Select(cfg => new Action(() =>
                  {
                      //Send the message.
                      Modules.MessageTemplates.WAR_Notifications.War_Started(cfg, WarNo).Wait();
                  })).executeParallel(bot.StopToken.Token, 1);

            foreach (Exception err in Errors)
            {
                await bot.Log.Error(null, err);
            }
        }

        public async Task SendPortalOpened()
        {
            Exception[] Errors = bot.GuildRepo
                 .GetCachedConfigs()
                 .Select(cfg => new Action(() =>
                 {
                     //Guild has elected out for this notification.
                     if (!cfg[Setting_Key.PORTAL_STARTED].Enabled || cfg.GetGuildChannel(WarBotChannelType.PORTAL) == null)
                         return;

                     //Send the message.
                     Modules.MessageTemplates.Portal_Notifications.Portal_Opened(cfg).Wait();
                 })).executeParallel(bot.StopToken.Token, 1);

            foreach (Exception err in Errors)
            {
                await bot.Log.Error(null, err);
            }
        }
    }
}

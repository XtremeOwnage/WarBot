using System;
using System.Collections.Generic;
using System.Text;
using WarBot.Core;

namespace WarBot.Storage.Scripted_Migrations
{
    public class _3_4_MoveNotificationsToGuildSettings
    {
        public static void Execute(WarDB db)
        {
            foreach (var guild in db.Guilds)
            {
                guild[Setting_Key.PORTAL_STARTED].Set(guild.NotificationSettings.PortalEnabled, guild.NotificationSettings.PortalStartedMessage);
                guild[Setting_Key.WAR_PREP_STARTED].Set(guild.NotificationSettings.WarPrepStarted, guild.NotificationSettings.WarPrepStartedMessage);
                guild[Setting_Key.WAR_PREP_ENDING].Set(guild.NotificationSettings.WarPrepEnding, guild.NotificationSettings.WarPrepEndingMessage);
                guild[Setting_Key.WAR_STARTED].Set(guild.NotificationSettings.WarStarted, guild.NotificationSettings.WarStartedMessage);

                guild[Setting_Key.WAR_1].Set(guild.NotificationSettings.War1Enabled, null);
                guild[Setting_Key.WAR_2].Set(guild.NotificationSettings.War2Enabled, null);
                guild[Setting_Key.WAR_3].Set(guild.NotificationSettings.War3Enabled, null);
                guild[Setting_Key.WAR_4].Set(guild.NotificationSettings.War4Enabled, null);

                guild[Setting_Key.USER_JOIN].Set(!string.IsNullOrEmpty(guild.NotificationSettings.GreetingMessage), guild.NotificationSettings.GreetingMessage);
                guild[Setting_Key.USER_LEFT].Set(guild.NotificationSettings.User_Left_Guild, null);

                guild[Setting_Key.WARBOT_UPDATES].Set(guild.NotificationSettings.SendUpdateMessage, null);
            }
        }
    }
}

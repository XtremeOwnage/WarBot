using Hangfire;
using System.Text;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.MessageTemplates
{
    public static class HustleWar
    {
        /// <summary>
        /// Sends an embed to the selected channel, if we have the proper permissions.
        /// Else- it will DM the owner of the guild.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="embed"></param>
        /// 
        /// <returns></returns>
        private static async Task sendHustleEventMessage(GuildLogic gLogic, HustleGuildChannelEventLogic eventConfig, string Message, bool IsFinalMessage = false)
        {
            if (!eventConfig.Enabled)
                return;

            ITextChannel? ch = await eventConfig.Channel.GetChannelAsync();

            //If there is no channel configured, abort.
            if (ch == null)
                return;

            //If we can send to the WAR channel, and we have permissions.
            if (await PermissionHelper.TestBotPermissionAsync(ch, ChannelPermission.SendMessages))
            {
                var sentMessage = await ch.SendMessageAsync(Message);

                Jobs.DeleteMessageJob.Enqueue(sentMessage, eventConfig, IsFinalMessage);

                return;
            }

            //Somebody should fix the permissions. Lets TRY to tell them.
            StringBuilder sb = new StringBuilder()
                .AppendLine("ERROR: Missing Permissions")
                .AppendLine($"You are receiving this error, because I do not have the proper permissions to send hustle war notification to channel {ch.Name}.")
                .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.")
                .AppendLine("Once you have corrected the permissions, you will need to re-enable this event.");

            BackgroundJob.Enqueue<Admin_Notifications>(o => o.SendMessage(ch.GuildId, sb.ToString()));

            eventConfig.Enabled = false;
            await eventConfig.GuildLogic.SaveChangesAsync();
        }

        public static async Task PrepStarted(GuildLogic cfg, HustleGuildChannelEventLogic warCfg)
        {
            //Guild has elected out for this notification.
            if (!warCfg.Enabled)
                return;

            ///Determine the message to send.
            string? Message = warCfg.Prep_Started_Message;
            if (!string.IsNullOrEmpty(Message))
                await sendHustleEventMessage(cfg, warCfg, Message);

        }
        public static async Task PrepEnding(GuildLogic cfg, HustleGuildChannelEventLogic warCfg)
        {
            //Guild has elected out for this notification.
            if (!warCfg.Enabled)
                return;

            ///Determine the message to send.
            string? Message = warCfg.Prep_Ending_Message;
            if (!string.IsNullOrEmpty(Message))
                await sendHustleEventMessage(cfg, warCfg, Message);

        }
        public static async Task EventStarted(GuildLogic cfg, HustleGuildChannelEventLogic warCfg)
        {
            //Guild has elected out for this notification.
            if (!warCfg.Enabled)
                return;

            ///Determine the message to send.
            string? Message = warCfg.Event_Started_Message;
            if (!string.IsNullOrEmpty(Message))
                await sendHustleEventMessage(cfg, warCfg, Message);
        }
        public static async Task EventFinished(GuildLogic cfg, HustleGuildChannelEventLogic warCfg)
        {
            //Guild has elected out for this notification.
            if (!warCfg.Enabled)
                return;

            ///Determine the message to send.
            string? Message = warCfg.Event_Finished_Message;
            if (!string.IsNullOrEmpty(Message))
                await sendHustleEventMessage(cfg, warCfg, Message);
        }
    }
}


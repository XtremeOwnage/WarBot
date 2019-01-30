using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Modules.MessageTemplates
{
    public static class WAR_Notifications
    {
        /// <summary>
        /// Sends an embed to the selected channel, if we have the proper permissions.
        /// Else- it will DM the owner of the guild.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="embed"></param>
        /// 
        /// <returns></returns>
        private static async Task sendWarMessage(IGuildConfig cfg, string Message)
        {
            SocketTextChannel ch = cfg.GetGuildChannel(WarBotChannelType.WAR) as SocketTextChannel;

            //If there is no channel configured, abort.
            if (ch == null)
                return;

            if (string.IsNullOrEmpty(Message))
                throw new NullReferenceException("War message is empty?");

            //If we can send to the WAR channel, and we have permissions.
            if (PermissionHelper.TestBotPermission(ch, ChannelPermission.SendMessages))
            {
                await ch.SendMessageAsync(Message);
                return;
            }

            //Handle missing permissions below this line.
            await cfg.Log.Error(cfg.Guild, new Exception($"Missing SEND_PERMISSIONS for channel {ch.Name} for guild {cfg.Guild.Name}"));

            StringBuilder sb = new StringBuilder()
                .AppendLine("ERROR: Missing Permissions")
                .AppendLine($"You are receiving this error, because I do not have the proper permissions to send the war notification to channel {ch.Name}.")
                .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.");

            bool messageSent = await cfg.Log.MessageServerLeadership(cfg, sb.ToString());

            //If we were unsuccessful in delivaring a message to the guid's leadership, disable the message.
            if (!messageSent)
            {
                //Well, out of options. Lets disable this channel for the guild.
                cfg.SetGuildChannel(WarBotChannelType.WAR, null);
                await cfg.SaveConfig();

                UnauthorizedAccessException error = new UnauthorizedAccessException("Missing permissions to send to WAR Channel. WAR messages disabled for this guild.");
                await cfg.Log.Error(cfg.Guild, error, nameof(sendWarMessage));
            }

        }

        /// <summary>
        /// Determine is a guild is elected into a specific war.           
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="WarNo"></param>
        /// <returns></returns>
        private static bool shouldSendSpecificWar(IGuildConfig cfg, byte WarNo)
        {
            if (WarNo == 1)
                return cfg[Setting_Key.WAR_1].Enabled;
            if (WarNo == 2)
                return cfg[Setting_Key.WAR_2].Enabled;
            if (WarNo == 3)
                return cfg[Setting_Key.WAR_3].Enabled;
            if (WarNo == 4)
                return cfg[Setting_Key.WAR_4].Enabled;

            else throw new ArgumentOutOfRangeException("There are only 4 wars. The value passed was not between 1 and 4.");
        }
        private static string getMessageForSpecificWar(string Input, byte WarNo)
        {
            if (Input.Split(';').Length == 4)
            {
                //Wars are 1-4, the array is indexed 0-3
                int index = WarNo - 1;

                //Get message for that specific war.
                return Input.Split(';')[index];
            }
            return Input;
        }
        public static async Task War_Prep_Started(IGuildConfig cfg, byte WarNo)
        {
            //Guild has elected out for this notification.
            if (!cfg[Setting_Key.WAR_PREP_STARTED].Enabled)
                return;
            //Guild elected out of this specific war.
            else if (!shouldSendSpecificWar(cfg, WarNo))
                return;

            ///Determine the message to send.
            string Message = "";
            if (cfg[Setting_Key.WAR_PREP_STARTED].HasValue)
            {
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_PREP_STARTED].Value, WarNo);

                if (!string.IsNullOrEmpty(Message))
                {
                    await sendWarMessage(cfg, Message);
                    return;
                }

                //If the message is empty- will revert to using the default message instead.
            }

            //Send a default message.
            if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                Message = $"{role.Mention}, WAR Placement has started! Please place your troops in the next two hours!";
            else
                Message = "WAR Placement has started! Please place your troops in the next two hours!";

            await sendWarMessage(cfg, Message);
        }
        public static async Task War_Prep_Ending(IGuildConfig cfg, byte WarNo)
        {
            //Guild has elected out for this notification.
            if (!cfg[Setting_Key.WAR_PREP_ENDING].Enabled)
                return;
            //Guild elected out of this specific war.
            else if (!shouldSendSpecificWar(cfg, WarNo))
                return;

            string Message = "";
            if (cfg[Setting_Key.WAR_PREP_ENDING].HasValue)
            {
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_PREP_ENDING].Value, WarNo);

                if (!string.IsNullOrEmpty(Message))
                {
                    await sendWarMessage(cfg, Message);
                    return;
                }
                //If the message is empty- will revert to using the default message instead.
            }

            //Send a default message.
            if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                Message = $"{role.Mention}, 15 minutes before war starts! Please place your troops if you have not done so already!!!";
            else
                Message = "15 minutes before war starts! Please place your troops if you have not done so already!!!";


            await sendWarMessage(cfg, Message);
        }
        public static async Task War_Started(IGuildConfig cfg, byte WarNo)
        {
            //Guild has elected out for this notification.
            if (!cfg[Setting_Key.WAR_STARTED].Enabled)
                return;
            //Guild elected out of this specific war.
            else if (!shouldSendSpecificWar(cfg, WarNo))
                return;

            ///Determine the message to send.
            string Message = "";

            if (cfg[Setting_Key.WAR_STARTED].HasValue)
            {
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_STARTED].Value, WarNo);

                if (!string.IsNullOrEmpty(Message))
                {
                    await sendWarMessage(cfg, Message);
                    return;
                }
                //If the message is empty- will revert to using the default message instead.
            }


            //Send a default message.
            if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                Message = $"{role.Mention}, WAR has started!";
            else
                Message = "WAR has started!";

            await sendWarMessage(cfg, Message);
        }

        /// <summary>
        /// This will bulk delete all messages from the clan's configured WAR Channel.
        /// Note- It will not delete pinned messages by design.
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static async Task clearWarChannel(IGuildConfig cfg)
        {
            SocketTextChannel ch = cfg.GetGuildChannel(WarBotChannelType.WAR) as SocketTextChannel;

            //If there is no channel configured, abort.
            if (ch == null)
                return;
            //Bot does not have permissions to manage messages.
            else if (!ch.TestBotPermission(ChannelPermission.ManageMessages))
            {
                StringBuilder sb = new StringBuilder()
                    .AppendLine("MISSING PERMISSIONS: MANAGE_MESSAGES")
                    .AppendLine("You have enabled the ability to clear war messages at the start of each ear.")
                    .AppendLine("But- I am missing the ability to manage messages for this channel.")
                    .AppendLine("I have automatically disabled this ability.")
                    .AppendLine()
                    .AppendLine("After you have corrected the missing permissions, you may re-enable this functionality by using:")
                    .AppendLine($"{cfg.Prefix} enable clear war channel");

                await cfg.Log.MessageServerLeadership(cfg, sb.ToString());

                cfg[Setting_Key.CLEAR_WAR_CHANNEL_ON_WAR_START].Disable();
                await cfg.SaveConfig();

                return;
            }

            DateTimeOffset discordBulkCutoffDate = DateTimeOffset.Now.AddDays(-13);

            //Loop through messages.
            while (true)
            {
                System.Collections.Generic.IAsyncEnumerable<System.Collections.Generic.IReadOnlyCollection<IMessage>> asyncresults = ch.GetMessagesAsync(500);
                System.Collections.Generic.IEnumerable<IMessage> results = await asyncresults.FlattenAsync();

                System.Collections.Generic.List<IMessage> ToBulkDelete = results
                    .Where(o => !o.IsPinned)
                    .Where(o => o.CreatedAt > discordBulkCutoffDate)
                    .ToList();

                try
                {
                    //If there are messages to bulk delete, do it.
                    if (ToBulkDelete.Count > 0)
                        await ch.DeleteMessagesAsync(ToBulkDelete);
                    else
                        break;
                }
                catch (Exception ex)
                {
                    await cfg.Log.Error(cfg.Guild, ex);

                    //Abort, after we have logged the error.
                    return;
                }
            }
        }
    }
}


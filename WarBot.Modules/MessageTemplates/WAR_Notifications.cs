using Discord;
using Discord.WebSocket;
using System;
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

            await cfg.Log.Error(cfg.Guild, new Exception($"Missing SEND_PERMISSIONS for channel {ch.Name} for guild {cfg.Guild.Name}"));

            StringBuilder sb = new StringBuilder()
                .AppendLine("ERROR: Missing Permissions")
                .AppendLine($"You are receiving this error, because I do not have the proper permissions to send the war notification to channel {ch.Name}.")
                .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.");

            //Else, we don't have permissions to the WAR Channel. Send a notification to the officers channel.
            SocketTextChannel och = cfg.GetGuildChannel(WarBotChannelType.CH_Officers) as SocketTextChannel;
            if (och != null && PermissionHelper.TestBotPermission(och, ChannelPermission.SendMessages))
            {
                await och.SendMessageAsync(sb.ToString());
                return;
            }

            //We don't have permissions to post to either channel. Lets try and DM the guild's owner... 
            try
            {
                IDMChannel dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(sb.ToString());
            }
            catch
            {
                //Well, out of options. Lets disable this channel for the guild.
                cfg.SetGuildChannel(WarBotChannelType.WAR, null);
                await cfg.SaveConfig();

                UnauthorizedAccessException error = new UnauthorizedAccessException("Missing permissions to send to WAR Channel. WAR messages disabled for this guild.");
                await cfg.Log.Error(cfg.Guild, error, nameof(sendWarMessage));

            }

        }

        private static string getMessageForSpecificWar(string Input, int WarNo)
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
        public static async Task War_Prep_Started(IGuildConfig cfg, int WarNo)
        {
            ///Determine the message to send.
            string Message = "";
            if (!cfg[Setting_Key.WAR_PREP_STARTED].HasValue)
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                    Message = $"{role.Mention}, WAR Placement has started! Please place your troops in the next two hours!";
                else
                    Message = "WAR Placement has started! Please place your troops in the next two hours!";
            else
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_PREP_STARTED].Value, WarNo);

            await sendWarMessage(cfg, Message);
        }
        public static async Task War_Prep_Ending(IGuildConfig cfg, int WarNo)
        {
            ///Determine the message to send.
            string Message = "";
            if (!cfg[Setting_Key.WAR_PREP_ENDING].HasValue)
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                    Message = $"{role.Mention}, 15 minutes before war starts! Please place your troops if you have not done so already!!!";
                else
                    Message = "15 minutes before war starts! Please place your troops if you have not done so already!!!";
            else
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_PREP_ENDING].Value, WarNo);

            await sendWarMessage(cfg, Message);
        }
        public static async Task War_Started(IGuildConfig cfg, int WarNo)
        {
            ///Determine the message to send.
            string Message = "";
            if (!cfg[Setting_Key.WAR_STARTED].HasValue)
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out IRole role) && role.IsMentionable)
                    Message = $"{role.Mention}, WAR has started!";
                else
                    Message = "WAR has started!";
            else
                Message = getMessageForSpecificWar(cfg[Setting_Key.WAR_STARTED].Value, WarNo);

            await sendWarMessage(cfg, Message);
        }
    }
}

using Discord;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Modules.MessageTemplates
{
    public static class Portal_Notifications
    {
        public static async Task Portal_Opened(IGuildConfig cfg)
        {
            ///Determine the message to send.
            string Message = "";
            if (!cfg[Setting_Key.PORTAL_STARTED].HasValue)
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out var role) && role.IsMentionable)
                    Message = $"{role.Mention}, The portal has opened!";
                else
                    Message = "The portal has opened!";
            else
                Message = cfg[Setting_Key.PORTAL_STARTED].Value;

            //Locate the portal channel.
            var ch = cfg.GetGuildChannel(WarBotChannelType.PORTAL) as SocketTextChannel;

            //If there is no channel configured, abort.
            if (ch == null)
                return;

            //Check if we can send to that channel.
            if (PermissionHelper.TestBotPermission(ch, ChannelPermission.SendMessages))
            {
                await ch.SendMessageAsync(Message);
            }
            else
            {
                Console.WriteLine($"Missing SEND_PERMISSIONS for channel {ch.Name} for guild {cfg.Guild.Name}");

                StringBuilder sb = new StringBuilder()
                    .AppendLine("ERROR: Missing Permissions")
                    .AppendLine($"You are receiving this error, because I do not have the proper permissions to send the notification to channel {ch.Name}.")
                    .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.");
                var result = await cfg.Log.MessageServerLeadership(cfg, sb.ToString());

                //If we were not able to delivar the message via officer channel, or DM, disable the portal messages automatically.
                if (!result)
                {
                    cfg[Setting_Key.PORTAL_STARTED].Disable();
                    await cfg.SaveConfig();
                }
            }
        }
    }
}

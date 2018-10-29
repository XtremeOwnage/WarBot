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
        /// <summary>
        /// Sends an embed to the selected channel, if we have the proper permissions.
        /// Else- it will DM the owner of the guild.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="embed"></param>
        /// 
        /// <returns></returns>
        private static async Task sendMessage(IGuildConfig cfg, Embed embed)
        {
            var ch = cfg.GetGuildChannel(WarBotChannelType.CH_WAR_Announcements) as SocketTextChannel;

            //If there is no channel configured, abort.
            if (ch == null)
                return;

            //Check if we can send to that channel.
            if (PermissionHelper.TestBotPermission(ch, ChannelPermission.SendMessages))
            {
                await ch.SendMessageAsync(embed: embed);
            }
            else
            {
                Console.WriteLine($"Missing SEND_PERMISSIONS for channel {ch.Name} for guild {cfg.Guild.Name}");
                //We don't have permissions to post to that channel. Lets DM the guild owner.
                var dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();

                StringBuilder sb = new StringBuilder()
                    .AppendLine("ERROR: Missing Permissions")
                    .AppendLine($"You are receiving this error, because I do not have the proper permissions to send the notification to channel {ch.Name}.")
                    .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.");

                await dm.SendMessageAsync(sb.ToString());
                await dm.SendMessageAsync(embed: embed);
            }
        }
        public static async Task Portal_Opened(IGuildConfig cfg)
        {
            ///Determine the message to send.
            string Message = "";
            if (string.IsNullOrEmpty(cfg.Notifications.PortalStartedMessage))
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out var role) && role.IsMentionable)
                    Message = $"{role.Mention}, The portal has opened!";
                else
                    Message = "The portal has opened!";
            else
                Message = cfg.Notifications.WarStartedMessage;

            var eb = new EmbedBuilder()
                .WithTitle("Portal")
                .WithDescription(Message);

            await sendMessage(cfg, eb.Build());
        }
    }
}

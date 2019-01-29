﻿using Discord;
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
        private static async Task sendMessage(IGuildConfig cfg, string Message)
        {
            var ch = cfg.GetGuildChannel(WarBotChannelType.WAR) as SocketTextChannel;

            //If there is no channel configured, abort.
            if (ch == null)
                return;

            if (string.IsNullOrEmpty(Message))
                throw new NullReferenceException("Portal message is empty.");

            //Check if we can send to that channel.
            if (PermissionHelper.TestBotPermission(ch, ChannelPermission.SendMessages))
            {
                await ch.SendMessageAsync(Message);
            }
            else
            {
                try
                {
                    Console.WriteLine($"Missing SEND_PERMISSIONS for channel {ch.Name} for guild {cfg.Guild.Name}");
                    //We don't have permissions to post to that channel. Lets DM the guild owner.
                    var dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();

                    StringBuilder sb = new StringBuilder()
                        .AppendLine("ERROR: Missing Permissions")
                        .AppendLine($"You are receiving this error, because I do not have the proper permissions to send the notification to channel {ch.Name}.")
                        .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.");

                    await dm.SendMessageAsync(sb.ToString());
                }
                catch(Exception ex)
                {
                    await cfg.Log.Error(cfg.Guild, ex, "SendMessage_Portal");

                    //Disable portal messages for this guild.
                    cfg[Setting_Key.PORTAL_STARTED].Disable();

                    await cfg.SaveConfig();
                }
            }
        }
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

            await sendMessage(cfg, Message);
        }
    }
}

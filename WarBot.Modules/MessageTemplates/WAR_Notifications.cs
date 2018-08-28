using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Modules.MessageTemplates
{
    public static class WAR_Notifications
    {
        public static async Task War_Prep_Started(IGuildConfig cfg, ITextChannel ch)
        {
            if (ch == null)
                return;

            ///Determine the message to send.
            string Message = "";
            if (string.IsNullOrEmpty(cfg.Notifications.WarPrepStartedMessage))
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out var role) && role.IsMentionable)
                    Message = $"{role.Mention}, WAR Placement has started! Please please your troops in the next two hours!";
                else
                    Message = "WAR Placement has started! Please please your troops in the next two hours!";
            else
                Message = cfg.Notifications.WarPrepStartedMessage;

            var eb = new EmbedBuilder()
                .WithTitle("WAR Prep Started")
                .WithDescription(Message);

            await ch.SendMessageAsync(embed: eb.Build());
        }

        public static async Task War_Prep_Ending(IGuildConfig cfg, ITextChannel ch)
        {
            if (ch == null)
                return;

            ///Determine the message to send.
            string Message = "";
            if (string.IsNullOrEmpty(cfg.Notifications.WarPrepEndingMessage))
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out var role) && role.IsMentionable)
                    Message = $"{role.Mention}, 15 minutes before war starts! Please place your troops if you have not done so already!!!";
                else
                    Message = "15 minutes before war starts! Please place your troops if you have not done so already!!!";
            else
                Message = cfg.Notifications.WarPrepEndingMessage;

            var eb = new EmbedBuilder()
                .WithTitle("WAR Prep Ending")
                .WithDescription(Message);

            await ch.SendMessageAsync(embed: eb.Build());
        }

        public static async Task War_Started(IGuildConfig cfg, ITextChannel ch)
        {
            if (ch == null)
                return;

            ///Determine the message to send.
            string Message = "";
            if (string.IsNullOrEmpty(cfg.Notifications.WarStartedMessage))
                if (cfg.GetGuildRole(RoleLevel.Member).IsNotNull(out var role) && role.IsMentionable)
                    Message = $"{role.Mention}, WAR has started!";
                else
                    Message = "WAR has started!";
            else
                Message = cfg.Notifications.WarStartedMessage;

            var eb = new EmbedBuilder()
                .WithTitle("WAR Started")
                .WithDescription(Message);

            await ch.SendMessageAsync(embed: eb.Build());
        }
    }
}

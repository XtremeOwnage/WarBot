using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    /// <summary>
    /// This module is responsible for allow the configuration of war notification related settings.
    /// </summary>
    public class PortalModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Leader)]
        [Command("set portal")]
        [Summary("This will set the message displayed when the portal is opened, and enable the notification.")]
        [CommandUsage("{prefix} {command} @Role, The portal has opened!")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetPortalOpen([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            this.cfg.Notifications.PortalEnabled = true;
            this.cfg.Notifications.PortalStartedMessage = Message;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable portal")]
        [Summary("Disables notifications when portal opens")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisablePortal()
        {
            cfg.Notifications.PortalEnabled = false;
            await cfg.SaveConfig();
            await ReplyAsync("I will no longer send a message when the portal is opened.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable portal")]
        [Summary("Enables notifications when portal opens.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnablePortal()
        {
            cfg.Notifications.PortalEnabled = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will send a message when the portal is opened.");
        }
    }
}
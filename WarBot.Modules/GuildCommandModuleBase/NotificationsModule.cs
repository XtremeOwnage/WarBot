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
    public class NotificationsModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Leader)]
        [Command("disable all notifications")]
        [Summary("This will disable all notifications from me.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableALLNotifications()
        {
            cfg[Setting_Key.WAR_PREP_STARTED].Enabled = false;
            cfg[Setting_Key.WAR_PREP_ENDING].Enabled = false;
            cfg[Setting_Key.WAR_STARTED].Enabled = false;
            cfg[Setting_Key.USER_JOIN].Enabled = false;
            cfg[Setting_Key.USER_LEFT].Enabled = false;
            cfg[Setting_Key.WARBOT_UPDATES].Enabled = false;
            cfg[Setting_Key.PORTAL_STARTED].Enabled = false;
            await cfg.SaveConfig();

            await ReplyAsync("All notifications have been disabled.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable all notifications")]
        [Summary("This will enable all notifications from me.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableALLNotifications()
        {
            cfg[Setting_Key.WAR_PREP_STARTED].Enabled = true;
            cfg[Setting_Key.WAR_PREP_ENDING].Enabled = true;
            cfg[Setting_Key.WAR_STARTED].Enabled = true;
            cfg[Setting_Key.USER_JOIN].Enabled = true;
            cfg[Setting_Key.USER_LEFT].Enabled = true;
            cfg[Setting_Key.WARBOT_UPDATES].Enabled = true;
            cfg[Setting_Key.PORTAL_STARTED].Enabled = true;
            await cfg.SaveConfig();

            await ReplyAsync("All notifications have been enabled.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable update notifications")]
        [Summary("This will disable a notification when I am updated")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableUPdateN()
        {
            cfg[Setting_Key.WARBOT_UPDATES].Enabled = false;
            await cfg.SaveConfig();
            await ReplyAsync("I will no longer alert you when I have new features added.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable update notifications")]
        [Summary("This will enable a notification when I am updated")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableUpdateN()
        {
            cfg[Setting_Key.WARBOT_UPDATES].Enabled = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will alert you when I have new features added or have been updated.");
        }

    }
}
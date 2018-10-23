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
    public class WarMessagesModule : GuildCommandModuleBase
    {
        #region Commands to SET messages
        [RoleLevel(RoleLevel.Leader)]
        [Command("set war prep started")]
        [Summary("This message is triggered when preperation peroid starts for clan wars.")]
        [CommandUsage("{prefix} {command} @Role, Please place your troops!!")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarPrepStarted([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            this.cfg.Notifications.WarPrepStartedMessage = Message;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war prep ending")]
        [Summary("This message is triggered 15 minutes before the war starts.")]
        [CommandUsage("{prefix} {command} @Role, War is starting soon!")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarPrepEnding([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            this.cfg.Notifications.WarPrepEndingMessage = Message;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war started")]
        [Summary("This message is triggered when the war starts")]
        [CommandUsage("{prefix} {command} @Role, War has started.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarStarted([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            this.cfg.Notifications.WarStartedMessage = Message;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }
        #endregion
        #region Commands to CLEAR/RESET messages
        [RoleLevel(RoleLevel.Leader)]
        [Command("set war prep started")]
        [Summary("Reset the war prep started message to defaults.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ClearWarPrepStarted()
        {
            this.cfg.Notifications.WarPrepStartedMessage = null;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war prep ending")]
        [Summary("Reset the war prep ending message to defaults.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ClearWarPrepEnding()
        {
            this.cfg.Notifications.WarPrepEndingMessage = null;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war started")]
        [Summary("Reset the war started message to defaults.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ClearWarStarted()
        {
            this.cfg.Notifications.WarStartedMessage = null;
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }
        #endregion
        #region Enable / Disable notifications


        #endregion
        [RoleLevel(RoleLevel.Leader)]
        [Command("test messages")]
        [Summary("This will send a test of each configured war message.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task TestMessages()
        {
            await MessageTemplates.WAR_Notifications.War_Prep_Started(this.cfg);
            await MessageTemplates.WAR_Notifications.War_Prep_Ending(this.cfg);
            await MessageTemplates.WAR_Notifications.War_Started(this.cfg);
        }

    }
}
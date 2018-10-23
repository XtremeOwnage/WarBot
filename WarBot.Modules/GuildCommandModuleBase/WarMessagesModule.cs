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
        #region Enable / Disable Specific war notifications
        [RoleLevel(RoleLevel.Leader)]
        [Command("disable war")]
        [Summary("Disables notifications for a specific war. 1=2amCST, 2 = 8amCST, 3 = 2pmCST, 4 = 8pmCST")]
        [CommandUsage("{prefix} {command} 1|2|3|4")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableWar(int War)
        {
            switch (War)
            {
                case 1:
                    cfg.Notifications.War1Enabled = false;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 2:
                    cfg.Notifications.War2Enabled = false;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 3:
                    cfg.Notifications.War3Enabled = false;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 4:
                    cfg.Notifications.War4Enabled = false;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                default:
                    await ReplyAsync("Please select between 1 and 4. 1 is for 2am central standard time war, while 4 is 8pm CST.");
                    break;
            }
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable war")]
        [Summary("Enables notifications for a specific war. 1=2amCST, 2 = 8amCST, 3 = 2pmCST, 4 = 8pmCST")]
        [CommandUsage("{prefix} {command} 1|2|3|4")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableWar(int War)
        {
            switch (War)
            {
                case 1:
                    cfg.Notifications.War1Enabled = true;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 2:
                    cfg.Notifications.War2Enabled = true;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 3:
                    cfg.Notifications.War3Enabled = true;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 4:
                    cfg.Notifications.War4Enabled = true;
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                default:
                    await ReplyAsync("Please select between 1 and 4. 1 is for 2am central standard time war, while 4 is 8pm CST.");
                    break;
            }
        }

        #endregion
        #region Enable / Disable Specific Notifications
        [RoleLevel(RoleLevel.Leader)]
        [Command("disable war prep started")]
        [Summary("Disables notifications when war prep starts.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableWarPrepStarted()
        {
            cfg.Notifications.WarPrepStarted = false;
            await cfg.SaveConfig();
            await ReplyAsync("I will no longer send a message when war prep starts.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable war prep started")]
        [Summary("Enables notifications when war prep starts.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableWarPrepStarted()
        {
            cfg.Notifications.WarPrepStarted = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will send a message when the war prep starts.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable war prep ending")]
        [Summary("Disables notifications when war prep is ending.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableWarPrepEnding()
        {
            cfg.Notifications.WarPrepEnding = false;
            await cfg.SaveConfig();
            await ReplyAsync("I will no longer send a message 15 minutes before war starts");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable war prep ending")]
        [Summary("Enables notification 15 minutes before end of war")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableWarPrepEnding()
        {
            cfg.Notifications.WarPrepEnding = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will send messages 15 minutes before war starts");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable war started")]
        [Summary("Disables notifications when war starts.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableWarStarted()
        {
            cfg.Notifications.WarStarted = false;
            await cfg.SaveConfig();
            await ReplyAsync("I will no longer send notifications when the war starts.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("enable war started")]
        [Summary("Enable notification when war starts.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableWarStarted()
        {
            cfg.Notifications.WarStarted = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will send notifications when the war starts.");
        }
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
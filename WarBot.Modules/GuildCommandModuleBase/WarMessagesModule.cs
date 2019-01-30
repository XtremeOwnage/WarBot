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
        [Summary("Enable and set the message when war preperation starts.")]
        [CommandUsage("{prefix} {command} @Role, Please place your troops!!")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarPrepStarted([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            cfg[Setting_Key.WAR_PREP_STARTED].Set(true, Message);
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war prep ending")]
        [Summary("Enable and set the message 15 minutes before war starts.")]
        [CommandUsage("{prefix} {command} @Role, War is starting soon!")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarPrepEnding([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            cfg[Setting_Key.WAR_PREP_ENDING].Set(true, Message);
            await cfg.SaveConfig();
            await ReplyAsync("Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("set war started")]
        [Summary("Enable and set the message when war starts.")]
        [CommandUsage("{prefix} {command} @Role, War has started.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWarStarted([Remainder] string Message)
        {
            if (Message.Length < 1)
                Message = null;

            cfg[Setting_Key.WAR_STARTED].Set(true, Message);
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
            cfg[Setting_Key.WAR_PREP_STARTED].Set(true, null);
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
            cfg[Setting_Key.WAR_PREP_ENDING].Set(true, null);
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
            cfg[Setting_Key.WAR_STARTED].Set(true, null);
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
                    cfg[Setting_Key.WAR_1].Disable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 2:
                    cfg[Setting_Key.WAR_2].Disable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 3:
                    cfg[Setting_Key.WAR_3].Disable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 4:
                    cfg[Setting_Key.WAR_4].Disable();
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
                    cfg[Setting_Key.WAR_1].Enable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 2:
                    cfg[Setting_Key.WAR_2].Enable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 3:
                    cfg[Setting_Key.WAR_3].Enable();
                    await cfg.SaveConfig();
                    await ReplyAsync("Done.");
                    break;
                case 4:
                    cfg[Setting_Key.WAR_4].Enable();
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
            cfg[Setting_Key.WAR_PREP_STARTED].Disable();
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
            cfg[Setting_Key.WAR_PREP_STARTED].Enable();
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
            cfg[Setting_Key.WAR_PREP_ENDING].Disable();
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
            cfg[Setting_Key.WAR_PREP_ENDING].Enable();
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
            cfg[Setting_Key.WAR_STARTED].Disable();
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
            cfg[Setting_Key.WAR_STARTED].Enable();
            await cfg.SaveConfig();
            await ReplyAsync("I will send notifications when the war starts.");
        }
        #endregion

        [RoleLevel(RoleLevel.Leader)]
        [Command("test war messages")]
        [Summary("This will send a test of each configured war message.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task TestMessages()
        {
            try
            {
                if (cfg[Setting_Key.WAR_PREP_STARTED]?.Value?.Split(';').Length == 4)
                    for (int i = 1; i < 5; i++)
                        await MessageTemplates.WAR_Notifications.War_Prep_Started(cfg, i);
                else
                    await MessageTemplates.WAR_Notifications.War_Prep_Started(cfg, 1);

                if (cfg[Setting_Key.WAR_PREP_ENDING]?.Value?.Split(';').Length == 4)
                    for (int i = 1; i < 5; i++)
                        await MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg, i);
                else
                    await MessageTemplates.WAR_Notifications.War_Prep_Ending(cfg, 1);

                if (cfg[Setting_Key.WAR_STARTED]?.Value?.Split(';').Length == 4)
                    for (int i = 1; i < 5; i++)
                        await MessageTemplates.WAR_Notifications.War_Started(cfg, i);
                else
                    await MessageTemplates.WAR_Notifications.War_Started(cfg, 1);
            }
            catch (System.Exception ex)
            {
                await bot.Log.Error(cfg.Guild, ex, "TestWarMessages");
            }
        }

    }
}
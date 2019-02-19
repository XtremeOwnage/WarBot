using Discord;
using Discord.Commands;
using Discord.WebSocket;
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

            cfg[Setting_Key.PORTAL_STARTED].Set(true, Message);
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
            cfg[Setting_Key.PORTAL_STARTED].Enabled = false;
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
            cfg[Setting_Key.PORTAL_STARTED].Enabled = true;
            await cfg.SaveConfig();
            await ReplyAsync("I will send a message when the portal is opened.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("test portal"), Alias("test portal messages")]
        [Summary("This will send a test of the portal messages")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task TestMessages()
        {
            try
            {
                ITextChannel ch = cfg.GetGuildChannel(WarBotChannelType.PORTAL);
                SocketTextChannel CH = ch as SocketTextChannel;

                if (ch == null)
                {
                    await ReplyAsync("You do not have a valid channel configured for sending PORTAL messages.\r\n" +
                        "Please configure a message using 'bot, set channel PORTAL #PortalChannel'");
                    return;
                }
                else if (!CH.TestBotPermission(ChannelPermission.ViewChannel))
                {
                    await ReplyAsync($"I do not have the READ_MESSAGES permission for {CH.Mention}\r\n" +
                        $"Please grant me the proper permissions and try again.");
                    return;
                }
                else if (!CH.TestBotPermission(ChannelPermission.SendMessages))
                {
                    await ReplyAsync($"I do not have the SEND_MESSAGES permission for {CH.Mention}\r\n" +
                        $"Please grant me the proper permissions and try again.");
                    return;
                }

                //Send the portal message.
                await MessageTemplates.Portal_Notifications.Portal_Opened(cfg);
            }
            catch (System.Exception ex)
            {
                await bot.Log.Error(cfg.Guild, ex, "TestPortalMessages");
            }
        }
    }
}
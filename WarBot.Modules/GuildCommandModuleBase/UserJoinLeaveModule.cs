using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    public class UserJoinLeaveModule : GuildCommandModuleBase
    {      
        [RoleLevel(RoleLevel.Leader)]
        [Command("enable leave"), Alias("set leave")]
        [Summary("Enables message to a specific channel when users leave a discord guild.")]
        [CommandUsage("{prefix} {command} #Channel")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableUserLeft(SocketTextChannel Channel)
        {
            //Update the config.
            cfg.SetGuildChannel(WarBotChannelType.CH_User_Left, Channel);
            cfg.Notifications.User_Left_Guild = true;

            await cfg.SaveConfig();

            await ReplyAsync($"Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable leave")]
        [Summary("Disable the message when users leave the server")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableLeave()
        {
            cfg.Notifications.User_Left_Guild = false;            
            await cfg.SaveConfig();

            await ReplyAsync($"The message has been disabled.");
        }

        
        [RoleLevel(RoleLevel.ServerAdmin)]
        [Command("setup")]
        [Summary("Starts the warbot configuration dialog.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnterSetup()
        {
            await this.bot.OpenDialog(new Dialogs.SetupDialog(this.Context));
        }
    }
}

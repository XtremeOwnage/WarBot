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
        #region User Left
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
        #endregion
        #region User Joined
        [RoleLevel(RoleLevel.Leader)]
        [Command("enable greeting"), Alias("set greeting")]
        [Summary("Enables message to a specific channel when users leave a discord guild.")]
        [CommandUsage("{prefix} {command} #Channel Your Message Goes Here")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableGreeting(SocketTextChannel Channel = null, [Remainder] string Message = null)
        {
            //Update the config.
            if (Channel != null)
                cfg.SetGuildChannel(WarBotChannelType.CH_User_Join, Channel);

            var ch = cfg.GetGuildChannel(WarBotChannelType.CH_User_Join);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                cfg.Notifications.User_Join_Guild = true;
                cfg.Notifications.NewUserGreeting = Message;
                await ReplyAsync($"I will greet new users in {ch.Mention} with the message you provided.");
            }
            else
            {
                cfg.Notifications.User_Join_Guild = true;
                cfg.Notifications.NewUserGreeting = null;
                await ReplyAsync($"I will greet new users in {ch.Mention} with a default message.");
            }

            await cfg.SaveConfig();

            await ReplyAsync($"Done.");
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("disable greeting")]
        [Summary("Disable the message when users leave the server")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task DisableGreeting()
        {
            cfg.Notifications.User_Left_Guild = false;
            await cfg.SaveConfig();

            await ReplyAsync($"The message has been disabled.");
        }
        #endregion

    }
}

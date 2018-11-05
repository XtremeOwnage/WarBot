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
            cfg.SetGuildChannel(WarBotChannelType.USER_LEFT, Channel);
            cfg[Setting_Key.USER_LEFT].Enable();

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
            cfg[Setting_Key.USER_LEFT].Disable();
            await cfg.SaveConfig();

            await ReplyAsync($"The message has been disabled.");
        }
        #endregion
        [RoleLevel(RoleLevel.Leader)]
        [Command("enable greeting"), Alias("set greeting")]
        [Summary("Enables message  when users leave a discord guild.")]
        [CommandUsage("{prefix} {command} Your Message Goes Here")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnableGreeting_MsgOnly([Remainder] string Message = null) => await EnableGreeting(null, Message);

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
                cfg.SetGuildChannel(WarBotChannelType.USER_JOIN, Channel);

            var ch = cfg.GetGuildChannel(WarBotChannelType.USER_JOIN);

            if (!string.IsNullOrWhiteSpace(Message))
            {
                cfg[Setting_Key.USER_JOIN].Set(true, Message);
                await ReplyAsync($"I will greet new users in {ch.Mention} with the message you provided.");
            }
            else
            {
                cfg[Setting_Key.USER_JOIN].Set(true, null);
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
            cfg[Setting_Key.USER_JOIN].Disable();
            await cfg.SaveConfig();

            await ReplyAsync($"The notification has been disabled.");
        }
        #endregion

    }
}

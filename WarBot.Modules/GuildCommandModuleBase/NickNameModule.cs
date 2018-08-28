using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    public class NickNameModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Officer)]
        [Command("set nickname"), Alias("nickname")]
        [Summary("Change WARBot's nickname.")]
        [CommandUsage("{prefix}, set nickname (NickName)")]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        public async Task SetNickName_Me(string Nickname)
        {
            var Me = Context.Guild.CurrentUser;

            if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
            {
                await ReplyAsync("The provided nickname was not valid.");
                return;
            }

            //Update the config.
            cfg.NickName = Nickname;
            await cfg.SaveConfig();

            //Update my nickname.
            await Me.ModifyAsync(o =>
            {
                o.Nickname = Nickname;
            });

            await ReplyAsync($"I have changed my nickname.");
        }

        [RoleLevel(RoleLevel.Officer)]
        [Command("set nickname"), Alias("nickname")]
        [Summary("Change a user's nickname.")]
        [CommandUsage("{prefix}, set nickname @User (NickName)")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task SetNickName_Other(SocketGuildUser user, string Nickname)
        {
            var Me = Context.Guild.CurrentUser;
            if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
            {
                await ReplyAsync("The provided nickname was not valid.");
                return;
            }

            //If WarBot was tagged, lets call the method to update his nickname.
            if (user.Id == Me.Id)
            {
                await SetNickName_Me(Nickname);
                return;
            }
            //Do a permissions check.
            else if (user.Hierarchy > Me.Hierarchy)
            {
                await ReplyAsync($"The target user is a member of a higher role then I am. I cannot modify or manage that user.");
            }
            else
            {
                await user.ModifyAsync(o =>
                {
                    o.Nickname = Nickname;
                });

                await ReplyAsync($"I have changed {user.Mention}'s nickname.");
            }
        }
    }
}
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    [RequireContext(ContextType.Guild)]
    public class NickNameModule : ModuleBase
    {
        private IGuildConfigRepository repo;
        public NickNameModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }

        [Command("set nickname"), Alias("nickname")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Change WARBot's nickname.")]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        public async Task SetNickName_Me(string Nickname)
        {
            var Me = (await this.Context.Guild.GetCurrentUserAsync()) as SocketGuildUser;
            if (Me == null)
                throw new System.NullReferenceException("Unable to find ME.");

            if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
            {
                await ReplyAsync("The provided nickname was not valid.");
                return;
            }

            //Update the config.
            var cfg = await repo.GetConfig(Context.Guild);
            cfg.NickName = Nickname;
            await cfg.SaveConfig();

            //Update my nickname.
            await Me.ModifyAsync(o =>
            {
                o.Nickname = Nickname;
            });

            await ReplyAsync($"I have changed my nickname.");
        }

        [Command("set nickname"), Alias("nickname")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Change a user's nickname.")]
        [RequireBotPermission(GuildPermission.ManageNicknames)]
        public async Task SetNickName_Other(SocketGuildUser user, string Nickname)
        {
            var Me = (await this.Context.Guild.GetCurrentUserAsync()) as SocketGuildUser;
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
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using System.Linq;


namespace WarBot.Modules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class TemplateModule : ModuleBase<SocketCommandContext>
    {
        private IGuildConfigRepository repo;
        public TemplateModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }

        [Command("command_goes_here"), Alias("add_alias")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Change WARBot's nickname.")]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        public async Task TemplateTask([Remainder]string Nickname)
        {
            var Me = Context.Guild.CurrentUser;
            var user = Context.User as SocketGuildUser;

            if (user == null)
                throw new System.NullReferenceException("User was not socket guild user.");

            //If user is ME(WarBot)
            if (Context.User.Id == Me.Id)
                await ReplyAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
            //Check if the user is a higher permission-level then WarBot - Will prevent warbot from managing target user.
            if (user.Hierarchy > Me.Hierarchy)
            {
                await ReplyAsync($"The target user is a member of a higher role then I am. I cannot kick that user.");
                return;
            }

            if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
            {
                await ReplyAsync("The provided nickname was not valid.");
                return;
            }


            //Get the stored guild config.
            var cfg = await repo.GetConfig(Context.Guild);

        }
    }
}
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    /// <summary>
    /// Contains simple messages, with little to no logic.
    /// </summary>
    /// 
    [RequireContext(ContextType.Guild)]
    public class EchoModule : GuildCommandModuleBase
    {
        [Command("say"), Summary("Echos a message."), RequireBotPermission(Discord.GuildPermission.SendMessages)]
        [RoleLevel(RoleLevel.None)]
        [CommandUsage("{prefix} say {Your text here}")]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("mimic me"), Summary("I will repeat everything you say, until you say stop.")]
        [RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(RoleLevel.None)]
        public async Task MimicMe()
        {
            await this.bot.OpenDialog(new Dialogs.MimicMeDialog(this.Context));
        }

        [Command("ping"), Summary("I will return a pong.")]
        [RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(RoleLevel.None)]
        public async Task AdminPing()
        {
            if (this.UserRole == RoleLevel.GlobalAdmin)
                await ReplyAsync("**Admin Pong**");
            else
                await ReplyAsync("**Pong**");
        }


    }
}

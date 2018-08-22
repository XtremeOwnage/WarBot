using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
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
        [Command("say"), Summary("Echos a message."), RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(Core.RoleLevel.None)]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("mimic me"), Summary("I will repeat everything you say, until you say stop."), RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(Core.RoleLevel.None)]
        public async Task MimicMe()
        {
            await this.bot.OpenDialog(new Dialogs.MimicMeDialog(this.Context));
        }

        [Command("ping"), Summary("Provides an Admin Pong"), RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(Core.RoleLevel.GlobalAdmin), Priority(5)]
        public async Task AdminPing()
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("**Pong**");
        }

        [Command("ping"), Summary("Provides an Admin Pong"), RequireBotPermission(Discord.GuildPermission.SendMessages), RoleLevel(Core.RoleLevel.None), Priority(0)]
        public async Task Pong()
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("***Pong***");
        }

    }
}

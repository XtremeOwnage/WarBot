using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.CommandModuleBase
{
    public class ServerAdminModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("go die"), Alias("restart"), Summary("Will force the WarBot process to stop, allowing it to be restarted.")]
        [CommandUsage("{prefix} go die")]
        public async Task GoDie()
        {

            var eb = new EmbedBuilder()
                .WithTitle("😭 GoodBye World 😭")
                .WithDescription("I am terminating my process now. I shall hopefully return.");

            await ReplyAsync(embed: eb.Build());
            System.Environment.Exit(0);
        }
    }
}

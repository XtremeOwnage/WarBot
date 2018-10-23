using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.CommandModuleBase
{
    /// <summary>
    /// Contains simple messages, with little to no logic.
    /// </summary>

    public class EchoModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [Command("say"), Summary("Echos a message.")]
        [Alias("echo", "repeat")]
        [CommandUsage("{prefix} say {Your text here}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Say([Remainder, Summary("The text to echo")] string echo)
        {
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(echo);
        }

        [Command("mimic me"), Summary("I will repeat everything you say, until you say stop.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task MimicMe()
        {
            await this.bot.OpenDialog(new Dialogs.MimicMeDialog(this.Context));
        }

        [Command("ping"), Summary("I will return a pong.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task AdminPing()
        {
            await ReplyAsync("**Pong**");
        }


    }
}

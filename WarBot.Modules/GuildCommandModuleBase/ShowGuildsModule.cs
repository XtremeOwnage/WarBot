using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;

namespace WarBot.Modules.CommandModules
{
    public class ShowGuildsModule : WarBot.Core.ModuleType.GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("show guilds"), Alias("list guilds")]
        [Summary("Prints out a list of all guilds utilizing warbot, and their member counts.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ShowGuilds()
        {
            System.Collections.Generic.IReadOnlyCollection<IGuild> Guilds = await bot.Client.GetGuildsAsync();

            StringBuilder sb = new StringBuilder()
                .AppendLine("Current Guilds And Member Count Using WarBot")
                .AppendLine("```");

            foreach (SocketGuild g in Guilds.OfType<SocketGuild>().OrderByDescending(o => o.MemberCount))
            {
                sb.AppendLine($"{g.MemberCount.ToString().PadRight(4, ' ')}\t{g.Name}");

                //Discord's api has a max length allowed. Once we get near this length, we need to send the message and start formatting a new message.
                if (sb.Length > 1900)
                {
                    ///Close the "Code" block.
                    sb.AppendLine("```");

                    //Send the current string buffer.
                    await ReplyAsync(sb.ToString());

                    //Clear the current buffer, and re-open a new "code" block.
                    sb.Clear()
                        .AppendLine("```");
                }
            }
            sb.AppendLine("```");

            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(sb.ToString());
        }
    }
}

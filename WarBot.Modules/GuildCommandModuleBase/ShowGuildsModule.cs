using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Discord.WebSocket;
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
            var Guilds = await bot.Client.GetGuildsAsync();

            StringBuilder sb = new StringBuilder()
                .AppendLine("Current Guilds And Member Count Using WarBot")
                .AppendLine("```");

            foreach (SocketGuild g in Guilds.OfType<SocketGuild>().OrderByDescending(o => o.MemberCount))
                sb.AppendLine($"{g.MemberCount.ToString().PadRight(4, ' ')}\t{g.Name}");
            sb.AppendLine("```");

            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(sb.ToString());
        }
    }
}

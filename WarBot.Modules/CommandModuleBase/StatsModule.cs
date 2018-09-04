using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WarBot.Modules.CommandModules
{
    public class StatsModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [Command("stats"), Alias("uptime", "show stats", "show uptime"), Summary("Display command stats related to me.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Stats_GUILD()
        {
            var Guilds = await bot.Client.GetGuildsAsync();
            TimeSpan ts = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            var eb = new EmbedBuilder()
                .WithTitle("WarBOT Statistics")
                .AddField("Process Uptime", $"{ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds")
                .AddField("Loaded Commands", bot.LoadedCommands, true)
                .AddField("Loaded Modules", bot.LoadedModules, true)
                .AddField("Messages Processed", bot.MessagesProcessed, true)
                .AddField("Guilds using WarBOT", Guilds.Count)
                .AddField("Environment", bot.Environment.ToString(), true);

            // ReplyAsync is a method on ModuleBase
            await ReplyAsync(embed: eb.Build());
        }
    }
}

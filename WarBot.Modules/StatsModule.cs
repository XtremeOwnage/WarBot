using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Modules
{
    public class StatsModule : ModuleBase<SocketCommandContext>
    {
        private IWARBOT bot;
        public StatsModule(IWARBOT bot)
        {
            this.bot = bot;
        }

        [Command("stats"), Alias("uptime", "show stats", "show uptime"), Summary("Display command stats related to me.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireContext(ContextType.Guild)]
        public async Task Stats_GUILD()
        {
            var Guilds = await bot.Client.GetGuildsAsync();
            TimeSpan ts = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            var eb = new EmbedBuilder()
                .WithTitle("WarBOT Statistics")
                .AddField("Process Uptime", $"{ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds")
                .AddInlineField("Loaded Commands", bot.LoadedCommands).AddInlineField("Loaded Modules", bot.LoadedModules)
                .AddInlineField("Messages Processed", bot.MessagesProcessed)
                .AddField("Guilds using WarBOT", Guilds.Count)
                .AddInlineField("Environment", bot.Environment.ToString());

            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("", embed: eb);
        }
    }
}

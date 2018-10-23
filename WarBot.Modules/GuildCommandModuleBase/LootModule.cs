using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    public class LootModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Leader)]
        [Command("set loot"), Alias("loot")]
        [Summary("Sets the loot for this guild.")]
        [CommandUsage("{prefix} {command} (Loot Details)")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetLoot([Remainder] string Loot)
        {
            cfg.Loot = Loot;
            await cfg.SaveConfig();
            await ReplyAsync("The loot value has been set.");
        }

        [RoleLevel(RoleLevel.Member)]
        [Command("show loot"), Alias("loot")]
        [Summary("Display this guild's loot instructions, if set.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ShowLoot()
        {
            if (string.IsNullOrEmpty(cfg.Loot))
            {
                await ReplyAsync("Sorry, your leader has not set a value for this command yet.");
            }
            else
            {
                await ReplyAsync(cfg.Loot);
            }
        }


    }
}
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules.GuildCommandModules
{
    [RequireContext(ContextType.Guild)]
    public class LootModule : GuildCommandModuleBase
    {
        [Command("set loot"), Alias("loot")]
        [RoleLevel(RoleLevel.Leader)]
        [Summary("Sets the loot for this guild.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetLoot([Remainder] string Loot)
        {
            cfg.Loot = Loot;
            await cfg.SaveConfig();
            await ReplyAsync("The loot value has been set.");
        }

        [Command("show loot"), Alias("loot")]
        [RoleLevel(RoleLevel.Member)]
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
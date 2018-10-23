using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class ResetConfigModule : GuildCommandModuleBase
    {
        [Command("reset config")]
        [Summary("Reset warbot's configuration to defaults.")]
        [CommandUsage("{prefix} {command}")]
        [RoleLevel(RoleLevel.Leader, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(GuildPermission.SendMessages)]
        public async Task ResetConfig()
        {
            await cfg.SetDefaults(Context.Guild);
            await ReplyAsync("All of my settings have been reverted to default.");
        }
    }
}
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    public class WebsiteModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Leader)]
        [Command("set website"), Alias("Website")]        
        [Summary("Sets the website for this guild.")]
        [CommandUsage("{prefix} {command} (WebsiteURL)")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWebsite([Remainder()] string Website)
        {
            cfg.Website = Website;
            await cfg.SaveConfig();
            await ReplyAsync("The website value has been set.");
        }

        [Command("show website"), Alias("Website")]
        [RoleLevel(RoleLevel.None)]
        [Summary("Display this guild's website, if set.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Website()
        {
            if (string.IsNullOrEmpty(cfg.Website))
            {
                await ReplyAsync("Sorry, your leader has not set a value for this command yet.");
            }
            else
            {
                await ReplyAsync(cfg.Website);
            }
        }
    }
}
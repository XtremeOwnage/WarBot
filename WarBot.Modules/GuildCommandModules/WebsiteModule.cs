using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules.GuildCommandModules
{
    [RequireContext(ContextType.Guild)]
    public class WebsiteModule : GuildCommandModuleBase
    {
        [Command("set website"), Alias("Website")]
        [RoleLevel(RoleLevel.Leader)]
        [Summary("Sets the website for this guild.")]
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
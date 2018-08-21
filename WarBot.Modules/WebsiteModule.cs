using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    [RequireContext(ContextType.Guild)]
    public class WebsiteModule : ModuleBase<SocketCommandContext>
    {
        private IGuildConfigRepository repo;
        public WebsiteModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }

        [Command("set website"), Alias("Website")]
        [RoleLevel(RoleLevel.Leader)]
        [Summary("Sets the website for this guild.")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetWebsite([Remainder()] string Website)
        {
            var cfg = await repo.GetConfig(this.Context.Guild);
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
            var cfg = await repo.GetConfig(this.Context.Guild);
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
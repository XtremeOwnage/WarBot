using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    [RequireContext(ContextType.Guild)]
    public class ShowConfigModule : ModuleBase
    {
        private IGuildConfigRepository repo;
        public ShowConfigModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }
        // ~say hello -> hello
        [Command("show config"), Alias("config show")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Display command stats related to me.")]
        [RequireBotPermission(Discord.ChannelPermission.SendMessages)]
        public async Task ShowConfig()
        {
            var cfg = await repo.GetConfig(this.Context.Guild);

            var eb1 = new EmbedBuilder()
                .WithTitle("Bot Configuration (1)")
                .AddField_ex("Admin Role", cfg.Role_Admin.Value?.Mention, true)
                .AddField_ex("Leader Role", cfg.Role_Leader.Value?.Mention, true)
                .AddField_ex("Officer Role", cfg.Role_Officer.Value?.Mention, true)
                .AddField_ex("Member Role", cfg.Role_Member.Value?.Mention, true)
                .AddBlankLine()
                .AddField_ex("Channel WAR Announcments", cfg.Channel_WAR_Notifications?.Value?.Mention)
                .AddField_ex("Channel Bot News / Updates", cfg.Channel_WarBot_News?.Value?.Mention)
                .AddField_ex("Channel Officer/Admin Messages", cfg.Channel_Officers?.Value?.Mention)
                .AddField_ex("Channel New User Greetings", cfg.Channel_NewUser_Welcome?.Value?.Mention)
                .AddBlankLine()
                .AddField_ex("Website Text", cfg.Website)
                .AddField_ex("Loot Text", cfg.Loot)
                .AddField_ex("My Nickname", cfg.NickName);

            var eb2 = new EmbedBuilder()
                .WithTitle("Bot Configuration (2)")
                .AddField_ex("War Prep Started Enabled", cfg.Notifications.WarPrepStarted)
                .AddField_ex("War Prep Started Message", cfg.Notifications.WarPrepStartedMessage)
                .AddField_ex("War Prep Ending Enabled", cfg.Notifications.WarPrepAlmostOver)
                .AddField_ex("War Prep Ending Message", cfg.Notifications.WarPrepEndingMessage)
                .AddField_ex("War Started Enabled", cfg.Notifications.WarStarted)
                .AddField_ex("War Started Message", cfg.Notifications.WarStartedMessage)
                .AddField_ex("War 1 Enabled (2am CST)", cfg.Notifications.War1Enabled)
                .AddField_ex("War 2 Enabled (8am CST)", cfg.Notifications.War2Enabled)
                .AddField_ex("War 3 Enabled (2pm CST)", cfg.Notifications.War3Enabled)
                .AddField_ex("War 4 Enabled (8pm CST)", cfg.Notifications.War4Enabled);
                

            try
            {
                // ReplyAsync is a method on ModuleBase
                await ReplyAsync("", embed: eb1);

                await ReplyAsync("", embed: eb2);
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
    }
}
using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    public class ServerAdminModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.ServerAdmin)]
        [Command("leave"), Alias("Go Away"), Summary("Warbot will leave the guild.")]
        [CommandUsage("{prefix} {command}")]
        public async Task Leave()
        {
            var eb = new EmbedBuilder()
                .WithTitle("GoodBye 😭")
                .WithDescription("I am sorry I did not meet the expectations of your guild. If you wish to invite me back, you may click this embed.")
                .WithUrl($"https://discordapp.com/oauth2/authorize?client_id={bot.Client.CurrentUser.Id}&scope=bot&permissions=0x00000008");


            if (PermissionHelper.TestBotPermission(Context.GuildChannel, ChannelPermission.SendMessages))
                await ReplyAsync(embed: eb.Build());
            else
            {
                var DM = await Context.GuildUser.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync(embed: eb.Build());
            }

            await Context.Guild.LeaveAsync();
        }


        [RoleLevel(RoleLevel.ServerAdmin)]
        [Command("set prefix")]
        [Summary("Change the prefix to address me.")]
        [CommandUsage("{prefix} {command} bot,")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task SetPrefix(string Prefix)
        {
            var Me = Context.Guild.CurrentUser;

            if (string.IsNullOrEmpty(Prefix))
            {
                await ReplyAsync("The provided prefix was not valid. The value has been set to 'bot,'");
                cfg.Prefix = "bot,";
                await cfg.SaveConfig();
                return;
            }

            //Update the config.
            cfg.Prefix = Prefix;
            await cfg.SaveConfig();

            await ReplyAsync($"My prefix has been set to '{Prefix}'");
        }

       
        [RoleLevel(RoleLevel.ServerAdmin)]
        [Command("setup")]
        [Summary("Starts the warbot configuration dialog.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task EnterSetup()
        {
            await this.bot.OpenDialog(new Dialogs.SetupDialog(this.Context));
        }
    }
}

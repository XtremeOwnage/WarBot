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
        [CommandUsage("{prefix} leave")]
        public async Task Leave()
        {

            var eb = new EmbedBuilder()
                .WithTitle("GoodBye 😭")
                .WithDescription("I am sorry I did not meet the expectations of your guild. If you wish to invite me back, you may click this embed.")
                .WithUrl("https://discordapp.com/oauth2/authorize?client_id=437983722193551363&scope=bot&permissions=0x00000008");


            if (PermissionHelper.TestBotPermission(Context.GuildChannel, ChannelPermission.SendMessages))
                await ReplyAsync("", embed: eb);
            else
            {
                var DM = await Context.GuildUser.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync("", embed: eb);
            }

            await Context.Guild.LeaveAsync();
        }


        [RoleLevel(RoleLevel.ServerAdmin)]
        [Command("set environment"), Summary("Choose the WARBot instance to use. This should only be used if you have specific instructions to use it.")]
        [CommandUsage("{prefix} set environment (PROD|NONPROD|LOCAL)")]
        public async Task SetEnvironment(string Environment)
        {
            if (System.Enum.TryParse(Environment, true, out WarBot.Core.Environment Env))
            {
                IGuildConfig CFG;
                if (this.cfg == null)
                    CFG = await bot.GuildRepo.GetConfig(Context.Guild, bot.Environment);
                else
                    CFG = cfg;

                //Update the config.
                CFG.Environment = Env;
                await CFG.SaveConfig();

                await ReplyAsync("This guild has been updated to use environment " + Env.ToString());
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                //Was unable to parse a role level from the input text.
                sb.AppendLine("I was unable to parse the desired environment from your input. The accepted values are:");
                var validValues = System.Enum.GetValues(typeof(WarBot.Core.Environment));
                foreach (Environment val in validValues)
                {
                    sb.AppendLine($"\t{val.ToString()}");
                }
                await ReplyAsync(sb.ToString());
            }
        }
    }
}

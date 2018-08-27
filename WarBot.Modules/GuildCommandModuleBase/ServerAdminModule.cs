using Discord;
using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    //All of these commands are server admin specific.
    [RoleLevel(RoleLevel.ServerAdmin)]
    public class ServerAdminModule : GuildCommandModuleBase
    {

        [Command("leave"), Alias("Go Away"), Summary("I will leave the guild."), RequireBotPermission(ChannelPermission.SendMessages), Priority(5)]
        public async Task Leave()
        {
            var eb = new EmbedBuilder()
                .WithTitle("GoodBye 😭")
                .WithDescription("I am sorry I did not meet the expectations of your guild. If you wish to invite me back, you may click this embed.")
                .WithUrl("https://discordapp.com/oauth2/authorize?client_id=437983722193551363&scope=bot&permissions=0x00000008");
            // ReplyAsync is a method on ModuleBase
            await ReplyAsync("", embed: eb);

            await Context.Guild.LeaveAsync();
        }

        [Command("leave"), Alias("Go Away"), Summary("I will leave the guild."), Priority(4)]
        public async Task Leave_NoMessage()
        {
            await Context.Guild.LeaveAsync();
        }

        [Command("set environment"), Summary("Choose the WARBot instance to use.")]
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

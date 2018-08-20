using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    //All of these commands are server admin specific.
    [RoleLevel(RoleLevel.ServerAdmin)]
    public class ServerAdminModule : ModuleBase
    {
        //Created via dependancy injection.
        public IGuildConfigRepository repo { get; set; }

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
    }
}

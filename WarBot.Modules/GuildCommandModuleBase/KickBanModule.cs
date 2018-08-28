using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    public class KickBanModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Leader)]
        [Command("kick"), Alias("remove")]       
        [Summary("Remove a user from this guild.")]
        [CommandUsage("{prefix} kick @User (Reason)")]
        [RequireBotPermission(GuildPermission.KickMembers | GuildPermission.SendMessages)]
        public async Task Kick(SocketGuildUser user, [Remainder]string Message = "An admin determined your services were no longer required.")
        {
            var Me = Context.Guild.CurrentUser;

            //Make sure to not kick myself.
            if (user.Id == Me.Id)
                await ReplyAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
            //Do a permissions check.
            else if(user.Hierarchy > Me.Hierarchy)
            {
                await ReplyAsync($"The target user is a member of a higher role then I am. I cannot kick that user.");
            }
            //Kick the user.
            else
            {
                await user.KickAsync(Message);
                var eb = new EmbedBuilder()
                    .WithTitle("User Kicked")
                    .WithColor(Color.Red)
                    .WithDescription($"User {user.Mention} has been removed from this guild.")
                    .AddField("Reason", Message);

                await ReplyAsync(embed: eb.Build());
            }
        }
    }
}
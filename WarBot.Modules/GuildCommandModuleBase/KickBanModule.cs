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
        [CommandUsage("{prefix} {command} @User (Reason)")]
        [RequireBotPermission(GuildPermission.KickMembers | GuildPermission.SendMessages)]
        public async Task Kick(SocketGuildUser user, [Remainder]string Message = "An admin determined your services were no longer required.")
        {
            var Me = Context.Guild.CurrentUser;

            //Make sure to not kick myself.
            if (user.Id == Me.Id)
                await ReplyAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
            //Do a permissions check.
            else if (user.Hierarchy > Me.Hierarchy)
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
                    .WithDescription($"User {user.Mention} has been removed from this guild by {Context.GuildUser.Mention}.")
                    .AddField("Reason", Message);

                await ReplyAsync(embed: eb.Build());
            }
        }

        [RoleLevel(RoleLevel.Leader)]
        [Command("ban")]
        [Summary("Remove a user from this guild.")]
        [CommandUsage("{prefix} {command} @User (Days = 365) (Reason)")]
        [RequireBotPermission(GuildPermission.BanMembers | GuildPermission.SendMessages)]
        public async Task Ban_1Year(SocketGuildUser user, [Remainder]string Message = "An admin determined your services were no longer required.")
            => await Ban(user, 365, Message);

        [RoleLevel(RoleLevel.Leader)]
        [Command("ban")]
        [Summary("Remove a user from this guild.")]
        [CommandUsage("{prefix} {command} @User (Reason)")]
        [RequireBotPermission(GuildPermission.BanMembers | GuildPermission.SendMessages)]
        public async Task Ban(SocketGuildUser user, int Days = 365, [Remainder]string Message = "An admin determined your services were no longer required.")
        {
            var Me = Context.Guild.CurrentUser;

            if (user.Id == Me.Id)
                await ReplyAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
            //Do a permissions check.
            else if (user.Hierarchy > Me.Hierarchy)
            {
                await ReplyAsync($"The target user is a member of a higher role then I am. I cannot ban that user.");
            }
            else
            {
                await user.BanAsync(Days, Message);
                var eb = new EmbedBuilder()
                    .WithTitle("User Banned")
                    .WithColor(Color.Red)
                    .WithDescription($"User {user.Mention} has been banned from this guild by {Context.GuildUser.Mention}.")
                    .AddField("Reason", Message)
                    .AddField("Days", Days);

                await ReplyAsync(embed: eb.Build());
            }
        }
    }
}
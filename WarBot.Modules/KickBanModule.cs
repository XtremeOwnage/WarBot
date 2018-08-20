﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    [RequireContext(ContextType.Guild)]
    public class KickBanModule : ModuleBase
    {
        private IGuildConfigRepository repo;
        public KickBanModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }

        [Command("kick"), Alias("remove")]
        [RoleLevel(RoleLevel.Leader)]
        [Summary("Remove a user from this guild.")]
        [RequireBotPermission(GuildPermission.KickMembers)]
        public async Task Kick(SocketGuildUser user, [Remainder]string Message = "An admin determined your services were no longer required.")
        {
            var Me = (await this.Context.Guild.GetCurrentUserAsync()) as SocketGuildUser;

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
                    .AddField_ex("Reason", Message);

                await ReplyAsync("", embed: eb);
            }
        }
    }
}
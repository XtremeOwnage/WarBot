using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.CommandModuleBase
{
    public class ServerAdminModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("go die"), Alias("restart"), Summary("Will force the WarBot process to stop, allowing it to be restarted.")]
        [CommandUsage("{prefix} go die")]
        public async Task GoDie()
        {

            var eb = new EmbedBuilder()
                .WithTitle("😭 GoodBye World 😭")
                .WithDescription("I am terminating my process now. I shall hopefully return.");

            await ReplyAsync(embed: eb.Build());
            System.Environment.Exit(0);
        }

        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("start job")]
        [Summary("Forces the scheduler to execute the provided job immediatly.")]
        [CommandUsage("{prefix} {command} (job name)")]
        public async Task InvokeJob(string Name)
        {
            try
            {
                await this.bot.Jobs.ExecuteJob(Name);
                await ReplyAsync("Done.");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("send pm")]
        [Summary("Sends a DM to another user.")]
        [CommandUsage("{prefix} {command} (person Id)")]
        public async Task Start_DM_ById(ulong Who, [Remainder] string Message)
        {
            try
            {
                var User = await this.bot.Client.GetUserAsync(Who) as SocketUser;
                if (User == null)
                    await ReplyAsync("Unable to find user by the ID provided");
                else
                    await Start_DM_ByTag(User, Message);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }

        [RoleLevel(RoleLevel.GlobalAdmin)]
        [Command("send pm")]
        [Summary("Establishes a group conversation between the requestor, and another person.")]
        [CommandUsage("{prefix} {command} @Person")]
        public async Task Start_DM_ByTag(SocketUser Who, [Remainder] string Message)
        {
            try
            {
                string from = $"{Context.User.Username}#{Context.User.Discriminator}";
                var dm = await Who.GetOrCreateDMChannelAsync();
                var m = await dm.SendMessageAsync($"New private message from {from}.\r\n" + Message);

                if (m != null)
                    await ReplyAsync("Success.");
                else
                    await ReplyAsync("Failure?");
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
        }



    }
}

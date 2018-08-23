using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Core.ModuleType;
using Humanizer;
using System;
using WarBot.Modules.TypeReaders;
using WarBot.Core;

//Disable async warning. Hangfire libarary will handle doing async automatically.
#pragma warning disable CS4014
namespace WarBot.Modules.CommandModules
{
    [Summary("Send a reminder message.")]
    public class RemindMeModule : CommandModuleBase
    {
        [Command("remind me"), Alias("remind")]
        public async Task RemindMe(TimeSpanext When, [Remainder]string Message)
        {

            //Check if we have permissions in this channel. If not, we will DM the user.
            if (this.Context is GuildCommandContext gcc && WarBot.Core.PermissionHelper.TestBotPermission(gcc, ChannelPermission.SendMessages))
            {
                await ReplyAsync($"I will remind you in {When.Span.Humanize()}, in this channel.");
                //ToDo - Fix job scheduling.
                await Task.Delay(When.Span);

                await gcc.GuildChannel.SendMessageAsync($"{gcc.User.Mention}, {Message}");
            }
            else
            {
                var DM = await Context.User.GetOrCreateDMChannelAsync();

                await DM.SendMessageAsync($"I will remind you in {When.Span.Humanize()}, via DM.");

                await Task.Delay(When.Span);

                await DM.SendMessageAsync($"Here is your remindar: {Message}");
            }

        }


    }

    public class RemindMeStandAloneJob
    {
        private IWARBOT bot;
        public RemindMeStandAloneJob(IWARBOT Bot)
        {
            this.bot = Bot;
        }

        public async Task SendReminder_DM(ulong UserId, string Message)
        {
            var User = await bot.Client.GetUserAsync(UserId);
            var Channel = await User.GetOrCreateDMChannelAsync();

            await Channel.SendMessageAsync($"Here is your remindar: {Message}");
        }
        public async Task SendReminder_GuildChannel(ulong UserId, ulong ChannelId, string Message)
        {
            var User = await bot.Client.GetUserAsync(UserId);
            var Channel = await bot.Client.GetChannelAsync(ChannelId);
            var GuildChannel = Channel as SocketTextChannel;

            await GuildChannel.SendMessageAsync($"{User.Mention}, Here is your remindar: {Message}");
        }

    }


}
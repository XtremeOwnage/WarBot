using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Core.ModuleType;
using Humanizer;
using System;
using WarBot.Core;

//Disable async warning. Hangfire libarary will handle doing async automatically.
#pragma warning disable CS4014
namespace WarBot.Modules.CommandModules
{
    [Summary("Send a reminder message.")]
    public class RemindMeModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [Command("remind me")]
        [Summary("Warbot will send you a remindar via this channel, after the timespan has passed.")]
        [CommandUsage("{prefix} {command} 1m Remind me about something in one minute!")]
        public async Task Remind_Me(TimeSpan When, [Remainder]string Message)
        {
            //Check if we have permissions in this channel. If not, we will DM the user.
            if (this.Context is GuildCommandContext gcc && WarBot.Core.PermissionHelper.TestBotPermission(gcc, ChannelPermission.SendMessages))
            {
                await ReplyAsync($"I will remind you here in {When.Humanize()}.");
                this.bot.Jobs.Schedule<RemindMeStandAloneJob>(o => o.SendReminder_GuildChannel_Me(gcc.User.Id, gcc.Channel.Id, Message), When);
            }
            else
            {
                var DM = await Context.User.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync($"I will remind you in {When.Humanize()}, via DM.");
                this.bot.Jobs.Schedule<RemindMeStandAloneJob>(o => o.SendReminder_DM(Context.User.Id, Message), When);
            }
        }

        [Command("remind dm")]
        [Summary("Warbot will send you a remindar via DM, after the timespan has passed.")]
        [CommandUsage("{prefix} {command} 1m Remind me about something in one minute!")]
        public async Task Remind_DM(TimeSpan When, [Remainder]string Message)
        {
            var DM = await Context.User.GetOrCreateDMChannelAsync();
            await DM.SendMessageAsync($"I will remind you in {When.Humanize()}, via DM.");
            this.bot.Jobs.Schedule<RemindMeStandAloneJob>(o => o.SendReminder_DM(Context.User.Id, Message), When);
        }

        [Command("remind here")]
        [Summary("Warbot will send the provided message to the current channel after the timespan has passed.")]
        [CommandUsage("{prefix} {command} 1m Remind here about about something in one minute!")]
        public async Task Remind_Here(TimeSpan When, [Remainder]string Message)
        {
            await ReplyAsync($"I will remind here in {When.Humanize()}.");
            this.bot.Jobs.Schedule<RemindMeStandAloneJob>(o => o.SendReminder_GuildChannel_Here(Context.Channel.Id, Message), When);
        }
    }

    /// <summary>
    /// Note- This service must be registered with the IServiceProvider for it to be properly 
    /// </summary>
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

            await Channel.SendMessageAsync($"{Message}");
        }
        public async Task SendReminder_GuildChannel_Me(ulong UserId, ulong ChannelId, string Message)
        {
            var User = await bot.Client.GetUserAsync(UserId);
            var Channel = await bot.Client.GetChannelAsync(ChannelId);
            var GuildChannel = Channel as SocketTextChannel;

            await GuildChannel.SendMessageAsync($"{User.Mention}, {Message}");
        }

        public async Task SendReminder_GuildChannel_Here(ulong ChannelId, string Message)
        {
            var Channel = await bot.Client.GetChannelAsync(ChannelId);
            var GuildChannel = Channel as SocketTextChannel;

            await GuildChannel.SendMessageAsync(Message);
        }

    }


}
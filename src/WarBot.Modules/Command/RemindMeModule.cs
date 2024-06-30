using Hangfire;
using Humanizer;
using WarBot.Modules.Jobs;

//Disable async warning. Hangfire libarary will handle doing async automatically.
#pragma warning disable CS4014
namespace WarBot.Modules.Command;

public class RemindMeModule : WarBOTModule
{

    [SlashCommand("remind", "Send a remindar after the specified duration.")]
    public async Task Remind_Me(
        [Summary("Delay", "How long to wait? ie, 5s for 5 seconds, 5m for 5 minutes, etc.")] TimeSpan When,
        [Summary("Message", "What message should I send?")] string Message,
        [Summary("WHere", "Where should the remindar be sent?")] RemindMeWHERE Where = RemindMeWHERE.Here)
    {
        bool CanSendHere = Context.Channel is ITextChannel gcc
            ? await gcc.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages)
            : true;

        //Check if we have permissions in this channel. If not, we will DM the user.
        if (Where == RemindMeWHERE.Here && CanSendHere)
        {
            await RespondAsync($"I will remind you here in {When.Humanize()}.");
            if (Context.Channel is IDMChannel)
                BackgroundJob.Schedule<RemindMeJob>(o => o.SendReminder_DM(this.Context.User.Id, Message), When);
            else
                BackgroundJob.Schedule<RemindMeJob>(o => o.SendReminder_GuildChannel_Here(this.Context.Channel.Id, Message), When);
        }
        else
        {
            var DM = await Context.User.CreateDMChannelAsync();
            await RespondAsync($"I will remind you in {When.Humanize()}, via DM.");
            BackgroundJob.Schedule<RemindMeJob>(o => o.SendReminder_DM(this.Context.User.Id, Message), When);
        }
    }

    public enum RemindMeWHERE
    {
        Here,
        DirectMessage
    }
}
using Hangfire;
using System.Text;

namespace WarBot.Modules.Command;

[TestGuildOnlyCommand]
[RoleLevel(RoleLevel.GlobalAdmin)]
[Group("admin", "Administrative commands")]
public class GlobalAdminModule : WarBOTModule
{
    private readonly ILogger<GlobalAdminModule> log;

    public GlobalAdminModule(ILogger<GlobalAdminModule> log)
    {
        this.log = log;
    }

    [SlashCommand("kill", "Will force the WarBot process to stop, allowing it to be restarted.")]
    public Task Kill() => GoDie();

    [SlashCommand("restart", "Will force the WarBot process to stop, allowing it to be restarted.")]
    public async Task GoDie()
    {

        var eb = new EmbedBuilder()
            .WithTitle("😭 GoodBye World 😭")
            .WithDescription("I am terminating my process now. I shall hopefully return.");

        await RespondAsync(embed: eb.Build());
        Environment.Exit(0);
    }

    [SlashCommand("announce", "Delivers a message to the configured update channel of all subscribed guilds.")]
    public async Task Announce(string Message, [Summary("Important", "If specified, will send regardless if guild has a configured update channel.")] bool Important = false)
    {
        try
        {
            await DeferAsync();

            await UseDbContextAsync(async db =>
            {
                if (Important)
                    foreach (var guild in Context.Client.Guilds)
                        BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(guild.Id, Message));
                else
                    foreach (var guild in Context.Client.Guilds)
                        BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendUpdate(guild.Id, Message));

            });

            await Context.Interaction.ModifyOriginalResponseAsync(o => o.Content = "Done.");
        }
        catch (Exception ex)
        {
            await RespondAsync(ex.Message);
        }
    }

    [SlashCommand("startjob", "Forces the scheduler to execute the provided job.")]
    public async Task InvokeJob(Models.JobList Job)
    {
        try
        {
            Hangfire.RecurringJob.Trigger(Job.ToString());
            await RespondAsync("Done.");
        }
        catch (Exception ex)
        {
            await RespondAsync(ex.Message);
        }
    }

    [SlashCommand("status", "Updates WARBot's status message.")]
    public async Task SetStatus([Summary("status", "New Status")] string Status)
    {
        try
        {
            await Context.Client.SetGameAsync(Status);
            await RespondAsync("Done.");
        }
        catch (Exception ex)
        {
            await RespondAsync(ex.Message);
        }
    }

    [SlashCommand("create_event", "Test Events.")]
    public async Task CreateTestingEvent()
    {
        await DeferAsync(UserOnly: true);

        await UseGuildLogicAsync(async logic =>
       {
           var evt = logic.HustleSettings.Event_War_1;

           var Event = await Context.Guild.CreateEventAsync(evt.Event_Title,
            startTime: DateTimeOffset.Now.AddMinutes(1),
            type: GuildScheduledEventType.External
            , GuildScheduledEventPrivacyLevel.Private
            , evt.Event_Description
            , DateTimeOffset.Now.AddHours(4)
            //, Context.Channel.Id
            , location: (await evt.Channel.GetChannelAsync()).Mention
    );





       });

        //var Event = await Context.Guild.CreateEventAsync("WarBOT v4.0 Development",
        //    startTime: DateTimeOffset.Now.AddMinutes(1),
        //    type: GuildScheduledEventType.External
        //    , GuildScheduledEventPrivacyLevel.Private
        //    , "Warbot v4.0 Development. Testing and development going on."
        //    , DateTimeOffset.Now.AddHours(4)
        //    //, Context.Channel.Id
        //    , location: "#testing"
        //    );

        //await Event.StartAsync();

        await RespondAsync("Event has been created. Let the development begin.");
    }

    [RoleLevel(RoleLevel.GlobalAdmin)]
    [SlashCommand("guilds", "Prints out a list of all guilds utilizing warbot, and their member counts.")]
    public async Task ShowGuilds()
    {
        IEnumerable<SocketGuild> Guilds = Context.Client.Shards.SelectMany(o => o.Guilds);

        StringBuilder sb = new StringBuilder()
            .AppendLine("Current Guilds And Member Count using WarBot")
            .AppendLine("```");

        foreach (SocketGuild g in Guilds.OrderByDescending(o => o.MemberCount))
        {
            sb.AppendLine($"{g.MemberCount.ToString().PadRight(4, ' ')}\t{g.Name}");

            //Discord's api has a max length allowed. Once we get near this length, we need to send the message and start formatting a new message.
            if (sb.Length > 1900)
            {
                ///Close the "Code" block.
                sb.AppendLine("```");

                //Send the current string buffer.
                await RespondAsync(sb.ToString());

                //Clear the current buffer, and re-open a new "code" block.
                sb.Clear()
                    .AppendLine("```");
            }
        }
        sb.AppendLine("```");

        // RespondAsync is a method on ModuleBase
        await RespondAsync(sb.ToString());
    }



    //[RoleLevel(RoleLevel.GlobalAdmin)]
    //[Command("send pm")]
    //[Summary("Sends a DM to another user.")]
    //[CommandUsage("{prefix} {command} (person Id)")]
    //public async Task Start_DM_ById(ulong Who, [Remainder] string Message)
    //{
    //    try
    //    {
    //        var User = this.Context.Client.GetUser(Who);
    //        if (User == null)
    //            await RespondAsync("Unable to find user by the ID provided");
    //        else
    //            await Start_DM_ByTag(User, Message);
    //    }
    //    catch (Exception ex)
    //    {
    //        await RespondAsync(ex.Message);
    //    }
    //}

    //[RoleLevel(RoleLevel.GlobalAdmin)]
    //[Command("send pm")]
    //[Summary("Establishes a group conversation between the requestor, and another person.")]
    //[CommandUsage("{prefix} {command} @Person")]
    //public async Task Start_DM_ByTag(SocketUser Who, [Remainder] string Message)
    //{
    //    try
    //    {
    //        string from = $"{Context.User.Username}#{Context.User.Discriminator}";
    //        var dm = await Who.CreateDMChannelAsync();
    //        var m = await dm.SendMessageAsync($"New private message from {from}.\r\n" + Message);

    //        if (m != null)
    //            await RespondAsync("Success.");
    //        else
    //            await RespondAsync("Failure?");
    //    }
    //    catch (Exception ex)
    //    {
    //        await RespondAsync(ex.Message);
    //    }
    //}
}

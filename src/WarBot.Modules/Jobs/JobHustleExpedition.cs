using WarBot.Data;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.Jobs;
/// <summary>
/// This class is called by the Job Scheduler.
/// </summary>
public class JobHustleExpedition
{
    private readonly IDiscordClient client;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger log;
    private const string noun = "expedition";

    public JobHustleExpedition(IDiscordClient client, IServiceProvider serviceProvider, ILogger<JobHustleExpedition> LOG)
    {
        this.client = client;
        this.serviceProvider = serviceProvider;
        log = LOG;
    }

    private static string DoReplacements(string Input, byte EventNo, HustleGuildChannelEventLogic logic)
    {
        return Input
            .Replace("{WAR_NO}", EventNo.ToString())
            .Replace("{CHANNEL}", logic.Channel.GetMention);
    }

    public async Task CreateDiscordEvents(byte EventNo)
    {
        log.LogDebug($"Sending discord events for {noun} {EventNo}");

        var timeHelper = WarTimeHelper.GetExpedition(EventNo);
        var start = WarTimeHelper.GetNextOccuranceDT(timeHelper.UTC_PrepStart_Hour);
        var end = WarTimeHelper.GetNextOccuranceDT(timeHelper.UTC_EventStart).AddMinutes(15);

        using (var scope = serviceProvider.CreateScope())
            foreach (var guild in await client.GetGuildsAsync())
                try
                {
                    using var db = scope.ServiceProvider.GetService<WarDB>();
                    using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                    var evt = logic.HustleSettings.GetExpedition(EventNo);
                    if (!evt.Enabled || !evt.Event_Enabled || !evt.Channel.ChannelID.HasValue)
                    {
                        log.LogDebug($"Guild {guild.Name} does not have discord events enabled.");
                        continue;
                    }

                    var Event = await guild.CreateEventAsync(
                        name: DoReplacements(evt.Event_Title, EventNo, evt),
                         startTime: start
                         , endTime: end
                         , type: GuildScheduledEventType.External
                         , privacyLevel: GuildScheduledEventPrivacyLevel.Private
                         , description: DoReplacements(evt.Event_Description, EventNo, evt)
                         , location: (await evt.Channel.GetChannelAsync()).Mention
                    );
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Error while sending discord event for guild {guild.Name}");
                }
    }

    public async Task SendWarPrepStarted(byte WarNo)
    {
        log.LogDebug($"Sending prep started for {noun} {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetExpedition(WarNo);
                await MessageTemplates.HustleExpedition.PrepStarted(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error while sending {noun} prep started.");
            }
    }

    public async Task SendWarPrepEnding(byte WarNo)
    {
        log.LogDebug($"Sending {noun} Prep Ending for War {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetExpedition(WarNo);
                await MessageTemplates.HustleExpedition.PrepEnding(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error while sending {noun} prep ending.");
            }
    }

    public async Task SendWarStarted(byte WarNo)
    {
        log.LogDebug($"Sending event started for {noun} {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetExpedition(WarNo);
                await MessageTemplates.HustleExpedition.EventStarted(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, $"Error while sending {noun} event started.");
            }
    }
}

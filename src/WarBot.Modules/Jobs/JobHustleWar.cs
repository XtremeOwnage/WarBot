using WarBot.Data;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.Jobs;
/// <summary>
/// This class is called by the Job Scheduler.
/// </summary>
public class JobHustleWar
{
    private readonly IDiscordClient client;
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<JobHustleWar> log;

    public JobHustleWar(IDiscordClient client, IServiceProvider serviceProvider, ILogger<JobHustleWar> LOG)
    {
        this.client = client;
        this.serviceProvider = serviceProvider;
        log = LOG;
    }

    private static string DoReplacements(string Input, byte War_No, HustleGuildChannelEventLogic logic)
    {
        return Input
            .Replace("{WAR_NO}", War_No.ToString())
            .Replace("{CHANNEL}", logic.Channel.GetMention);
    }

    public async Task CreateDiscordEvents(byte WarNo)
    {
        log.LogDebug($"Sending discord events for war {WarNo}");

        var timeHelper = WarTimeHelper.GetWar(WarNo);
        var start = WarTimeHelper.GetNextOccuranceDT(timeHelper.UTC_PrepStart_Hour);
        var end = start.AddHours(WarTimeHelper.PrepHoursBeforeStart).AddMinutes(15);
        log.LogDebug($"Current Time UTC: {DateTimeOffset.UtcNow:HH:mm}. War Prep Start Time: {start:HH:mm}. Event End Time: {end:HH:mm}");

        using (var scope = serviceProvider.CreateScope())
            foreach (var guild in await client.GetGuildsAsync())
                try
                {
                    using var db = scope.ServiceProvider.GetService<WarDB>();
                    using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                    var war = logic.HustleSettings.GetWar(WarNo);
                    if (!war.Enabled || !war.Event_Enabled || !war.Channel.ChannelID.HasValue)
                    {
                        log.LogDebug($"Guild {guild.Name} does not have discord events enabled.");
                        continue;
                    }

                    var Event = await guild.CreateEventAsync(
                        name: DoReplacements(war.Event_Title, WarNo, war),
                         startTime: start
                         , endTime: end
                         , type: GuildScheduledEventType.External
                         , privacyLevel: GuildScheduledEventPrivacyLevel.Private
                         , description: DoReplacements(war.Event_Description, WarNo, war)
                         , location: (await war.Channel.GetChannelAsync()).Mention
                    );
                }
                catch (Exception ex)
                {
                    log.LogError(ex, $"Error while sending war prep started for guild {guild.Name}");
                }
    }

    public async Task SendWarPrepStarted(byte WarNo)
    {
        log.LogDebug($"Sending War Prep Ended for War {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetWar(WarNo);
                await MessageTemplates.HustleWar.PrepStarted(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while sending war prep started.");
            }
    }

    public async Task SendWarPrepEnding(byte WarNo)
    {
        log.LogDebug($"Sending War Prep Ending for War {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetWar(WarNo);
                await MessageTemplates.HustleWar.PrepEnding(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while sending war prep ending.");
            }
    }

    public async Task SendWarStarted(byte WarNo)
    {
        log.LogDebug($"Sending War Started for War {WarNo}");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var war = logic.HustleSettings.GetWar(WarNo);
                await MessageTemplates.HustleWar.EventStarted(logic, war);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while sending war started.");
            }
    }

    public async Task SendPortalOpened()
    {
        log.LogDebug($"Sending Portal Opened Notification.");
        foreach (var guild in await client.GetGuildsAsync())
            try
            {
                using var scope = serviceProvider.CreateScope();
                using var db = scope.ServiceProvider.GetService<WarDB>();
                using var logic = await GuildLogic.GetOrCreateAsync(client, db, guild);
                var portal = logic.HustleSettings.Event_Portal;
                await MessageTemplates.HustlePortal.Portal_Opened(logic);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while sending portal opened");
            }
    }
}

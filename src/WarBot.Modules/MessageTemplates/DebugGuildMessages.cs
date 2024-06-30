using WarBot.Data;

namespace WarBot.Modules.MessageTemplates;

/// <summary>
/// This class is used to relay important messages to a guild's leadership or owner.
/// </summary>
public class DebugGuildMessages
{
    private readonly DiscordShardedClient client;
    private readonly ILogger<DebugGuildMessages> log;

    public DebugGuildMessages(DiscordShardedClient client, ILogger<DebugGuildMessages> log)
    {
        this.client = client;
        this.log = log;
    }

    public async Task GuildJoined(ulong Guild)
    {
        var guild = client.GetGuild(Guild);
        if (guild is null)
        {
            log.LogWarning("Unable to find guild by ID.");
            return;
        }


        log.LogInformation($"New Guild Added: {guild.Name}. Member Count: {guild.MemberCount}");
    }

    public async Task GuildLeft(string GuildName, int? Members)
    {
        log.LogInformation($"Guild Removed: {GuildName}. Member Count: {Members}");
    }
}

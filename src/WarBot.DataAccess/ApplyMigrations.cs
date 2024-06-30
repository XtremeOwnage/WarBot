using Microsoft.Extensions.Logging;
using WarBot.Core;

namespace WarBot.DataAccess;
public static class ApplyMigrations
{
    public static async Task Execute(WarDB db, ILogger log)
    {
        log.LogInformation("Running migrations.");

        var matchingGuilds = await db
            .Set<GuildSettings>()
            .Include(o => o.HustleCastleSettings)
            .Where(o => o.BotVersion < BotConfig.BOT_VERSION)
            .ToListAsync();


        foreach (var guild in matchingGuilds)
        {
            //Added Expeditions 
            if (guild.BotVersion < 4.1)
            {
                log.LogInformation($"Updating guild {guild.DiscordName} to version 4.1");
                guild.HustleCastleSettings.Expedition_1 = new HustleGuildChannelEvent();
                guild.HustleCastleSettings.Expedition_2 = new HustleGuildChannelEvent();
                guild.HustleCastleSettings.Expedition_3 = new HustleGuildChannelEvent();
                guild.HustleCastleSettings.Expedition_4 = new HustleGuildChannelEvent();

                //Set the bot version.
                guild.BotVersion = 4.1;
            }
        }

        log.LogInformation("Completed migrations. Saving changes");

        await db.SaveChangesAsync();

    }
}


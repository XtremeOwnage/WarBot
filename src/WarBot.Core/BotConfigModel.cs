using System.Collections.Generic;

namespace WarBot.Core;

public class BotConfigModel
{
    /// <summary>
    /// Array of user IDs with super admin privileges.
    /// </summary>
    public ulong[] SUPERADMIN_USER_IDS { get; set; }

    /// <summary>
    /// Array of guilds authorized for receiving admin commands.
    /// </summary>
    public ulong[] ADMIN_GUILDS { get; set; }

    /// <summary>
    /// The host address for the database.
    /// </summary>
    public string DB_HOST { get; set; }

    /// <summary>
    /// The port number for the database.
    /// </summary>
    public uint DB_PORT { get; set; }

    /// <summary>
    /// The name of the database.
    /// </summary>
    public string DB_NAME { get; set; }

    /// <summary>
    /// The username for the database.
    /// </summary>
    public string DB_USER { get; set; }

    /// <summary>
    /// The password for the database.
    /// </summary>
    public string DB_PASS { get; set; }

    /// <summary>
    /// The token for the Discord bot.
    /// </summary>
    public string DISCORD_TOKEN { get; set; }

    /// <summary>
    /// The client ID for the Discord bot.
    /// </summary>
    public ulong DISCORD_ID { get; set; }

    /// <summary>
    /// The client secret for the Discord bot.
    /// </summary>
    public string DISCORD_SECRET { get; set; }

    /// <summary>
    /// The public URL of the bot.
    /// </summary>
    public string PUBLIC_URL { get; set; }
}

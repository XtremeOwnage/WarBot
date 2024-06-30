using System;
using System.Collections.Generic;

namespace WarBot.Core;

/// <summary>
/// This class will store bot-specific configuration items needed at application startup.
/// </summary>
public static class BotConfig
{
    /// <summary>
    /// Array of user IDs with super admin privileges.
    /// </summary>
    public static ulong[] SUPERADMIN_USER_IDS { get; set; }

    /// <summary>
    /// Array of guilds authorized for receiving admin commands.
    /// </summary>
    public static ulong[] ADMIN_GUILDS { get; set; }

    /// <summary>
    /// The host address for the database.
    /// </summary>
    public static string DB_HOST { get; set; }

    /// <summary>
    /// The port number for the database.
    /// </summary>
    public static uint DB_PORT { get; set; }

    /// <summary>
    /// The name of the database.
    /// </summary>
    public static string DB_NAME { get; set; }

    /// <summary>
    /// The username for the database.
    /// </summary>
    public static string DB_USER { get; set; }

    /// <summary>
    /// The password for the database.
    /// </summary>
    public static string DB_PASS { get; set; }

    /// <summary>
    /// The token for the Discord bot.
    /// </summary>
    public static string DISCORD_TOKEN { get; set; }

    /// <summary>
    /// The client ID for the Discord bot.
    /// </summary>
    public static ulong DISCORD_ID { get; set; }

    /// <summary>
    /// The client secret for the Discord bot.
    /// </summary>
    public static string DISCORD_SECRET { get; set; }

    /// <summary>
    /// The public URL of the bot.
    /// </summary>
    public static string PUBLIC_URL { get; set; }

    #region Non-configurable or calculated properties.
    /// <summary>
    /// The version of the bot.
    /// </summary>
    public static double BOT_VERSION => 5.0;

    /// <summary>
    /// The list of permissions for the Discord bot.
    /// </summary>
    public static Discord.GuildPermission DISCORD_PERMISSIONS_LIST { get; } =
        Discord.GuildPermission.AddReactions
        | Discord.GuildPermission.BanMembers
        | Discord.GuildPermission.ChangeNickname
        | Discord.GuildPermission.CreatePublicThreads
        | Discord.GuildPermission.EmbedLinks
        | Discord.GuildPermission.KickMembers
        | Discord.GuildPermission.ManageEvents
        | Discord.GuildPermission.ManageMessages
        | Discord.GuildPermission.ManageNicknames
        | Discord.GuildPermission.ManageRoles
        | Discord.GuildPermission.MentionEveryone
        | Discord.GuildPermission.ModerateMembers
        | Discord.GuildPermission.ReadMessageHistory
        | Discord.GuildPermission.SendMessages
        | Discord.GuildPermission.ViewChannel;

    public static string DISCORD_INVITE_URL_FULL => $"https://discordapp.com/oauth2/authorize?client_id={DISCORD_ID}&scope=applications.commands%20bot&permissions={(ulong)DISCORD_PERMISSIONS_LIST}";

    public static string DISCORD_INVITE_URL_ADD_SCOPE_ONLY => $"https://discordapp.com/oauth2/authorize?client_id={DISCORD_ID}&scope=applications.commands";
    #endregion
}
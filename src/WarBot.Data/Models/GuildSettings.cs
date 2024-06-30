using System.Diagnostics;
using WarBot.Core;

namespace WarBot.Data.Models;
#nullable enable
/// <summary>
/// Represent a guild in discord.
/// </summary>
[DebuggerDisplay("Name = {Name}")]
public class GuildSettings : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    public ulong DiscordID { get; set; }
    public string DiscordName { get; set; } = string.Empty;

    /// <summary>
    /// Represents the latest version of WarBOT for which this guild was active.
    /// </summary>
    public double BotVersion { get; set; } = BotConfig.BOT_VERSION;

    /// <summary>
    /// The prefix to which warbot will respond to.
    /// </summary>
    public string BotPrefix { get; set; } = "bot,";

    /// <summary>
    /// What message to display when somebody request's the website.
    /// </summary>
    public string? Website { get; set; } = null;

    /// <summary>
    /// Guild's Timezone.
    /// </summary>
    public string? TimeZone { get; set; } = null;

    public virtual GuildChannel Channel_Admins { get; set; } = new GuildChannel();
    public virtual GuildChannelEvent Event_Updates { get; set; } = new GuildChannelEvent();
    public virtual GuildChannelEvent Event_UserJoin { get; set; } = new GuildChannelEvent();
    public virtual GuildChannelEvent Event_UserLeft { get; set; } = new GuildChannelEvent();
    public virtual HustleCastleSettings HustleCastleSettings { get; set; } = new HustleCastleSettings();
    public virtual GuildRoles Roles { get; set; } = new GuildRoles();

    public virtual ICollection<CustomSlashCommand> CustomCommands { get; set; } = new List<CustomSlashCommand>();
}

namespace WarBot.Data.Models;
/// <summary>
/// Represents a Discord Guild Channel, with a specific purpose.
/// </summary>
public class GuildRoles : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; internal set; }

    public virtual GuildRole Guest { get; set; } = new GuildRole();
    public virtual GuildRole Member { get; set; } = new GuildRole();
    public virtual GuildRole SuperMember { get; set; } = new GuildRole();
    public virtual GuildRole Officer { get; set; } = new GuildRole();
    public virtual GuildRole Leader { get; set; } = new GuildRole();
    public virtual GuildRole ServerAdmin { get; set; } = new GuildRole();
}


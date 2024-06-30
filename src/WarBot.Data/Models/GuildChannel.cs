namespace WarBot.Data.Models;
/// <summary>
/// Represents a Discord Guild Channel, with a specific purpose.
/// </summary>
public class GuildChannel : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; internal set; }
    public ulong? DiscordID { get; set; }
    public string? DiscordName { get; set; }
}


#nullable enable
namespace WarBot.Data.Models;
/// <summary>
/// This table holds settings specific to hustle castle.
/// </summary>
public class HustleCastleSettings : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    public string? LootMessage { get; set; } = null;

    public virtual GuildChannelEvent Portal { get; set; } = new GuildChannelEvent();

    public virtual HustleGuildChannelEvent War_1 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent War_2 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent War_3 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent War_4 { get; set; } = new HustleGuildChannelEvent();

    public virtual HustleGuildChannelEvent? Expedition_1 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent? Expedition_2 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent? Expedition_3 { get; set; } = new HustleGuildChannelEvent();
    public virtual HustleGuildChannelEvent? Expedition_4 { get; set; } = new HustleGuildChannelEvent();
}


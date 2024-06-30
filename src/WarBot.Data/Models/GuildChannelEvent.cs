#nullable enable
namespace WarBot.Data.Models;
/// <summary>
/// This table holds an event, for a guild channel.
/// </summary>
public class GuildChannelEvent : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    /// <summary>
    /// Is this event enabled?
    /// </summary>
    public bool Enabled { get; set; } = false;

    /// <summary>
    /// What Channel does this event target.
    /// </summary>
    public virtual GuildChannel Channel { get; set; } = new GuildChannel();

    /// <summary>
    /// What Message should be triggered from this event.
    /// </summary>
    public string? Message { get; set; } = null;

    /// <summary>
    /// If true, will create a discord event for this activity.
    /// </summary>
    public bool CreateEvent { get; set; } = false;

    /// <summary>
    /// Sets the event's title.
    /// </summary>
    public string? EventTitle { get; set; }

    /// <summary>
    /// Sets the event's message.
    /// </summary>
    public string? EventDescription { get; set; }

    /// <summary>
    /// How many minutes should messages be retained?
    /// </summary>
    public int? ClearAfterMins { get; set; } = null;

    /// <summary>
    /// Enum containing the method for clearing the war channel. Enum is defined in DataAccess library.
    /// </summary>
    public uint? ClearMethod { get; set; } = null;


}


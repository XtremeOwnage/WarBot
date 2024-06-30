namespace WarBot.Data.Models;
public class CustomCommandAction : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    /// <summary>
    /// Which entity owns this record?
    /// </summary>
    public CustomSlashCommand Parent { get; set; }


    /// <summary>
    /// A user-defined friendly name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// What type of custom command is this?
    /// </summary>
    public CustomCommandActionType Type { get; set; }

    /// <summary>
    /// If this is a SEND_MESSAGE command, what is the message?
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Target channel for this action.
    /// </summary>
    public GuildChannel TargetChannel { get; set; } = new GuildChannel();

    //Target role for this action.
    public GuildRole TargetRole { get; set; } = new GuildRole();

    /// <summary>
    /// ID of target role, channel, user.... etc....
    /// </summary>
    public ulong? ItemId { get; set; }


}


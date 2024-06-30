using WarBot.Core;

namespace WarBot.Data.Models;
public class CustomSlashCommand : IRecord
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long ID { get; set; }

    /// <summary>
    /// Reference to the parent record.
    /// </summary>
    public GuildSettings Parent { get; set; }

    public bool PublishSlashCommand { get; set; } = true;

    public string Name { get; set; } = String.Empty;

    public string Description { get; set; } = String.Empty;

    public RoleLevel? MinimumRoleLevel { get; set; }

    public ICollection<CustomCommandAction> Actions { get; set; } = new List<CustomCommandAction>();
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models
{
    /// <summary>
    /// Represents a entity in discord, identified by name and ID (ulong)
    /// </summary>
    public abstract class BaseDiscordEntity
    {
        /// <summary>
        /// Primary key. May me multiple channels used.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }
        /// <summary>
        /// Primary key, and discord entity ID.
        /// </summary>
        public ulong EntityId { get; protected set; }

        /// <summary>
        /// The Name identifying this entity.
        /// </summary>
        public string Name { get; set; }

    }
}

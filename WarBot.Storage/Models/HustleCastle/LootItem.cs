using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models.HustleCastle
{
    /// <summary>
    /// Represents an available Loot item, for WAR.
    /// </summary>
    public class LootItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Who won this piece of loot?
        /// </summary>
        public virtual HustleUser WinningUser { get; set; }
    }
}

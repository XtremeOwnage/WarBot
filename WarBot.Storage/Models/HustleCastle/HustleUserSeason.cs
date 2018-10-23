using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WarBot.Storage.Models.HustleCastle
{
    public class HustleUserSeason
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        /// <summary>
        /// Reference back to the Guild Season.
        /// </summary>
        public virtual HustleGuildSeason Season { get; set; }
        /// <summary>
        /// A reference to the Game user.
        /// </summary>
        public virtual HustleUser User { get; set; }

        /// <summary>
        /// How much glory did the user obtain for the season?
        /// </summary>
        public uint Glory { get; set; }

        /// <summary>
        /// What items has the user selected NEED for.
        /// </summary>
        public virtual List<LootItem> NeedItems { get; } = new List<LootItem>();

        /// <summary>
        /// What items has the user selected GREED for?
        /// </summary>
        public virtual List<LootItem> GreedItems { get; } = new List<LootItem>();
    }
}

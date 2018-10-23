using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Storage.Models.HustleCastle
{
    /// <summary>
    /// Represents a hustle-castle clan war season.
    /// </summary>
    public class HustleGuildSeason
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        /// <summary>
        /// Ending date for this season.
        /// </summary>
        public DateTime DateEnd { get; set; }

        /// <summary>
        /// Available loot.
        /// </summary>
        public virtual List<LootItem> AvailableLoot { get; } = new List<LootItem>();

        /// <summary>
        /// Minimum required glory specified by the GUILD to receive rewards.
        /// </summary>
        public uint MinimumGuildGlory { get; set; }

        /// <summary>
        /// Hustle castle has a specified minimum glory to receive resources as well.
        /// </summary>
        public uint MinimumGameGlory { get; set; }

        /// <summary>
        /// Reference to each user's season data.
        /// </summary>
        public virtual List<HustleUserSeason> UserData { get; } = new List<HustleUserSeason>();

        /// <summary>
        /// Is this season active? If False, this season will become readonly.
        /// </summary>
        public bool Active { get; set; }
    }
}

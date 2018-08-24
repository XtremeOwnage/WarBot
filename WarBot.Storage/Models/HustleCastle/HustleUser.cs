using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WarBot.Storage.Models.HustleCastle
{
    /// <summary>
    /// Represents a hustle-castle user account.
    /// </summary>
    public class HustleUser
    {
        public HustleUser() { }

        /// <summary>
        /// Primary key.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        /// <summary>
        /// The character's name.
        /// </summary>
        [Required, StringLength(20, MinimumLength = 2)]
        public string CharacterName { get; set; }

        [Range(1, 10, ErrorMessage = "Throne room must be between 1 and 10.")]
        /// <summary>
        /// What TR level is the user? TR1 - TR10
        /// </summary>
        /// 
        public byte ThroneRoomLevel { get; set; } = 0;

        /// <summary>
        /// What is the player's current squad power.
        /// </summary>
        public uint SquadPower { get; set; } = 0;

        [Required]
        /// <summary>
        /// Reference back to the warbot user / discord user.
        /// </summary>
        public virtual DiscordUser User { get; set; }

        public bool IsActiveCharacter { get; set; } = true;
        /// <summary>
        /// A reference to the season's this user has participated in.
        /// </summary>
        public virtual List<HustleGuildSeason> Seasons { get; } = new List<HustleGuildSeason>();
    }
}

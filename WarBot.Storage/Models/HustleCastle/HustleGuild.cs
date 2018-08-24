using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models.HustleCastle
{
    public class HustleGuild
    {
        /// <summary>
        /// Primary key.
        /// </summary>
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        /// <summary>
        /// The name of the hustle-castle guild.
        /// </summary>
        public string Name { get; set; }

        [ForeignKey("DiscordGuildId")]
        /// <summary>
        /// Reference to the discord guild.
        /// </summary>
        public virtual DiscordGuild DiscordGuild { get; set; }

        /// <summary>
        /// List of user's into the guild.
        /// </summary>
        public virtual List<HustleUser> Users { get; } = new List<HustleUser>();

        /// <summary>
        /// List of seasons.
        /// </summary>
        public virtual List<HustleGuildSeason> Seasons { get; } = new List<HustleGuildSeason>();
    }
}

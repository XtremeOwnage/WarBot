using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using WarBot.Storage.Models.HustleCastle;

namespace WarBot.Storage.Models
{
    /// <summary>
    /// Discord User.
    /// </summary>
    public class DiscordUser : BaseDiscordEntity
    {
        /// <summary>
        /// Empty constructor for EF.
        /// </summary>
        public DiscordUser() { }

        public DiscordUser(SocketUser User)
        {
            this.EntityId = User.Id;
            this.Name = User.Username;
        }
        public DiscordUser(IUser User)
        {
            this.EntityId = User.Id;
            this.Name = User.Username;
        }

        /// <summary>
        /// A reference to this user's characters. User may have multiple characters.
        /// </summary>
        public virtual List<HustleUser> Characters { get; } = new List<HustleCastle.HustleUser>();

        [NotMapped]
        /// <summary>
        /// This user's SELECTED character.
        /// </summary>
        public virtual HustleUser SelectedCharacter => Characters.FirstOrDefault(o => o.IsActiveCharacter == true);

        /// <summary>
        /// The last time this user was seen online.
        /// </summary>
        public DateTimeOffset? LastOnline { get; set; } = null;

        /// <summary>
        /// The last time a message was seen by this user.
        /// </summary>
        public DateTimeOffset? LastActivity { get; set; } = null;
    }
}

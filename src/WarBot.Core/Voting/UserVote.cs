using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Core.Voting
{
    /// <summary>
    /// Represents a user's vote in a guild.
    /// </summary>
    public class UserVote
    {
        /// <summary>
        /// Empty constructor required by EF.
        /// </summary>
        public UserVote() { }

        public UserVote(IUser User, PollOption Option)
        {
            this.User = User;
            this.UserId = User.Id;
            this.Option = Option;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        [Required, ForeignKey(nameof(Poll))]
        public int PollID { get; set; }

        public virtual Poll Poll { get; }

        [Required]
        public ulong UserId { get; }

        [NotMapped]
        public IUser User { get; internal set; }

        [Required, ForeignKey(nameof(Option))]
        public int OptionID { get; set; }

        public virtual PollOption Option { get; }
    }
}

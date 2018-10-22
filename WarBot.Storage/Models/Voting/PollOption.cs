using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WarBot.Storage.Models.Voting
{
    public class PollOption
    {
        /// <summary>
        /// Parameter-less consturctor for entity framework.
        /// </summary>
        public PollOption() { }

        public PollOption(string Name, IEmote Emote)
        {
            this.Name = Name;
            this.Emote = Emote;
        }


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        [Required, ForeignKey(nameof(Poll))]
        public int PollID { get; set; }

        public virtual Poll Poll { get; }

        /// <summary>
        /// The option's name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// This is the Emote value.
        /// </summary>
        [Required]
        public string Value { get; set; }
        /// <summary>
        /// The emote which represents this option.
        /// </summary>
        [NotMapped]
        public IEmote Emote
        {
            get => new Emoji(Value);
            set => Value = value.Name;
        }
    }
}

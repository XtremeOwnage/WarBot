using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;
namespace WarBot.Storage.Models
{
    /// <summary>
    /// Represents a Discord Guild Channel, with a specific purpose.
    /// </summary>
    public class GuildChannel : BaseDiscordEntity
    {
        public GuildChannel() { }
        public GuildChannel(WarBotChannelType Type, ITextChannel Channel)
        {
            this.ChannelType = Type;
            this.Set(Channel);
        }


        //Navigation Property.
        public virtual DiscordGuild Guild { get; set; }

        [Required]
        public WarBotChannelType ChannelType { get; set; }

        [NotMapped]
        public ITextChannel Value { get; private set; }

        public void Set(ITextChannel Value)
        {
            this.EntityId = Value.Id;
            this.Name = Value.Name;
            this.Value = Value;
        }
    }
}

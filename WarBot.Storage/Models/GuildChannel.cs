using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;
namespace WarBot.Storage.Models
{
    public class GuildChannel : DiscordEntity, IStoredDiscordEntity<ITextChannel>
    {
        public GuildChannel() { }
        public GuildChannel(WarBotChannelType Type, ITextChannel Channel)
        {
            this.ChannelType = Type;
            this.Set(Channel);
        }

        //Navigation Property.
        public virtual GuildConfig GuildConfig { get; set; }

        [Required]
        public WarBotChannelType ChannelType { get; set; }

        [NotMapped]
        public ITextChannel Value { get; set; }

        public void Set(ITextChannel Value)
        {
            this.EntityId = Value.Id;
            this.Name = Value.Name;
            this.Value = Value;
        }
    }
}

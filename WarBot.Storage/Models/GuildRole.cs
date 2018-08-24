using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;
namespace WarBot.Storage.Models
{
    public class GuildRole : BaseDiscordEntity
    {
        public GuildRole() { }

        public GuildRole(RoleLevel level, IRole Role)
        {
            this.Level = level;
            this.Set(Role);
        }

        [Required]
        public RoleLevel Level { get; set; }

        //Navigation Property.
        public virtual DiscordGuild Guild { get; set; }

        [NotMapped]
        public IRole Value { get; private set; }

        public void Set(IRole Value)
        {
            this.EntityId = Value.Id;
            this.Name = Value.Name;
            this.Value = Value;
        }
    }
}

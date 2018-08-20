using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;
namespace WarBot.Storage.Models
{
    public class DiscordEntity
    {
        public DiscordEntity() { }

        public static DiscordEntity CreateNew()
        {
            return new DiscordEntity
            {
                EntityId = null,
                Name = null
            };
        }


        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }
        public ulong? EntityId { get; private set; }

        public string Name { get; set; }

        [NotMapped]
        public object Value { get; set; }

        public IEntityStorage<T> GetEntity<T>() where T : IEntity<ulong>
        {
            if (Value != null)
                return new EntityStorage<T>(this.Name, (T)this.Value);
            else if (EntityId.HasValue)
                return new EntityStorage<T>(this.Name, this.EntityId.Value);
            else
                return null;
        }

        public void Set<T>(IEntityStorage<T> Value) where T : IEntity<ulong>
        {
            this.EntityId = Value.ID;
            this.Name = Value.Name;
            this.Value = Value.Value;
        }

        public void Set(ulong ID, string Name)
        {
            this.EntityId = ID;
            this.Name = Name;
            this.Value = null;
        }
    }
}

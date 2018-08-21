using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models
{
    public class DiscordEntity : IEntity<ulong>
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        #region Stored properties, for Discord
        public string Name { get; set; }
        public ulong? EntityId { get; protected set; }
        #endregion

        #region IEntity<ulong> implementation        
        ulong IEntity<ulong>.Id => EntityId.Value;
        #endregion


        //This is used to set the value, without actually having the value.... 
        //Such as, when migrating config versions to the latest version.
        public void Set(ulong ID, string Name)
        {
            this.EntityId = ID;
            this.Name = Name;
        }
    }
}

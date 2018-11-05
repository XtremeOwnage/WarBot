using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;

namespace WarBot.Storage.Models
{
    /// <summary>
    /// Represents a configurable setting, which holds a string and a bool.
    /// This is so that I do not have to perform a schema update, every time I wish to add a new setting, or change something.
    /// </summary>
    public class GuildSetting : IGuildSetting
    {
        public GuildSetting() { }
        public GuildSetting(Setting_Key Key)
        {
            this.Key = Key;
        }
        [Key]
        public int ID { get; private set; }
        public Setting_Key Key { get; private set; }

        public bool Enabled { get; set; } = false;
        public string Value { get; set; } = null;

        [NotMapped]
        public bool HasValue => !string.IsNullOrWhiteSpace(this.Value);

        public void Enable() => this.Enabled = true;
        public void Disable() => this.Enabled = false;
        public void Clear() => this.Value = null;
        //Navigation Property.
        public virtual DiscordGuild Guild { get; set; }

        public void Set(bool Enable, string Value)
        {
            this.Enabled = Enable;
            this.Value = Value;
        }
    }
}

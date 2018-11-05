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


        public bool Enable { get; set; } = false;
        public string Value { get; set; } = null;

        //Navigation Property.
        public virtual DiscordGuild Guild { get; set; }
    }
}

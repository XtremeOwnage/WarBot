using Newtonsoft.Json;
using System.Diagnostics;

namespace WarBot.UI.Models.Discord
{
    [DebuggerDisplay("{name}")]
    public class UserGuild
    {
        public string id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public bool? owner { get; set; }
        public Permissions Permissions { get; set; }
        // public int permissions { get; set; }
        public string[] features { get; set; }


        [JsonIgnore]
        public bool IsAdmin => Permissions.HasFlag(Permissions.ADMINISTRATOR);

        [JsonIgnore]
        public ulong? ID_NUM => ulong.TryParse(id, out ulong num) ? num : null;

        [JsonIgnore]
        public bool HasWarbotConfig { get; set; } = false;

    }
}

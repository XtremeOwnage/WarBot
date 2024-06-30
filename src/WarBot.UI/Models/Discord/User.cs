namespace WarBot.UI.Models.Discord
{
    public class User
    {
        public string id { get; set; }
        public string username { get; set; }
        public string discriminator { get; set; }
        public string avatar { get; set; }
        public bool? verified { get; set; }
        public string email { get; set; }
        public int? flags { get; set; }
        public string banner { get; set; }
        public int? accent_color { get; set; }
        public int? premium_type { get; set; }
        public int? public_flags { get; set; }

    }
}

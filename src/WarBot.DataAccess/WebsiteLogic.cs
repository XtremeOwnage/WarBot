namespace WarBot.DataAccess
{
    public class WebsiteLogic
    {
        private readonly WarDB db;

        public WebsiteLogic(WarDB db)
        {
            this.db = db;
        }

        public Task<bool> HasWarbotConfig(ulong GuildID)
        {
            return db.Set<GuildSettings>().AnyAsync(o => o.DiscordID == GuildID);
        }
    }
}

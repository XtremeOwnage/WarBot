using Humanizer;

namespace WarBot.Modules.Jobs
{
    public class UpdateStatus
    {
        private readonly DiscordShardedClient discordClient;
        private readonly ILogger<UpdateStatus> log;

        public UpdateStatus(DiscordShardedClient discordClient, ILogger<UpdateStatus> log)
        {
            this.discordClient = discordClient;
            this.log = log;
        }

        public async Task Execute(string Status = null)
        {
            try
            {
                await discordClient.SetStatusAsync(UserStatus.Online);

                //If no status was passed in, lets dynamically update the status based on the time.
                if (string.IsNullOrEmpty(Status))
                    switch (DateTime.UtcNow.Hour)
                    {
                        //In war prep.
                        case int i when i >= 1 && i < 3 || i >= 7 && i < 9 || i >= 13 && i < 15 || i >= 19 && i < 21:
                            {
                                var warStart = GetNextEvent(new int[] { 3, 9, 15, 21 });
                                var timeUntil = warStart.Subtract(DateTime.UtcNow);

                                Status = timeUntil.Humanize() + " until war.";
                            }
                            break;
                        case int i when (i == 3 || i == 9 || i == 15 || i == 21) && DateTime.Now.Minute < 10:
                            Status = "War is being fought now.";
                            break;
                        default:
                            {
                                //IDiscordClient c = Client as IDiscordClient;
                                //var Guilds = await c.GetGuildsAsync();
                                //var Members = Guilds.OfType<SocketGuild>().Sum(o => o.MemberCount);
                                //Status = $"Serving {Members} players in {Guilds.Count} guilds!";
                                var nextPrep = GetNextEvent(new int[] { 1, 7, 13, 19 });
                                var timeUntil = nextPrep.Subtract(DateTime.UtcNow);

                                Status = "Next prep in " + timeUntil.Humanize();
                            }
                            break;
                    }
                await discordClient.SetGameAsync(Status, null);
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to update current status.");
            }
        }

        public DateTime GetNextEvent(int[] Events)
        {
            var now = DateTime.UtcNow;

            if (now.Hour > Events.Max())
                return new DateTime(now.Year, now.Month, now.Day, Events.Min(), 0, 0, DateTimeKind.Utc).AddDays(1);
            else
                return new DateTime(now.Year, now.Month, now.Day, Events.First(o => o > now.Hour), 0, 0, DateTimeKind.Utc);
        }
    }
}
using Discord.Net;
using Hangfire;

namespace WarBot.Modules.Jobs
{
    public class CheckDiscordScopes
    {
        private readonly IDiscordClient client;
        private readonly ILogger log;

        public CheckDiscordScopes(IDiscordClient client, ILogger<CheckDiscordScopes> log)
        {
            this.client = client;
            this.log = log;
        }

        public async Task Execute()
        {
            foreach (var guild in await client.GetGuildsAsync())
            {
                try
                {
                    await guild.GetApplicationCommandsAsync();
                }
                catch (HttpException ex) when (ex.DiscordCode == DiscordErrorCode.MissingPermissions)
                {
                    log.LogError("Guild {guild.name} is missing application command permissions.", guild);

                    BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(guild.Id, "I am missing critical scopes for my operation. Please have an admin click this link to update my scopes: " + BotConfig.DISCORD_INVITE_URL_ADD_SCOPE_ONLY));
                    Console.WriteLine("Well then.");
                }

            }
        }
    }
}

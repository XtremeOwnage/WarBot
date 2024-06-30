using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;
using WarBot.UI.Models.Discord;

namespace WarBot.UI.Components
{
    public class DiscordAPI
    {
        private readonly DiscordRestClient discordBotClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IHttpClientFactory httpClientFactory;

        public DiscordAPI(DiscordRestClient discordBotClient, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
            this.discordBotClient = discordBotClient;
            this.httpContextAccessor = httpContextAccessor;
        }

        private async ValueTask<string?> getAccessToken()
        {
            var actoken = await httpContextAccessor.HttpContext.GetTokenAsync("access_token");
            return actoken;
        }
        private async Task<HttpClient> getClient()
        {
            var client = httpClientFactory.CreateClient("discord");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await getAccessToken());

            return client;
        }

        public async Task<List<UserGuild>> GetUserGuildsAsync()
        {
            using var userGuildsRequest = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me/guilds");
            using var client = await getClient();

            var userguildResponse = await client.SendAsync(userGuildsRequest);

            userguildResponse.EnsureSuccessStatusCode();

            var content = await userguildResponse.Content.ReadAsStringAsync();

            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserGuild>>(content);

            //var joinedGuilds = await discordBotClient.GetGuildsAsync();
            //foreach (var d in data.Where(o => o.ID_NUM.HasValue))
            //    d.HasWarbotConfig = joinedGuilds.Any(o => o.Id == d.ID_NUM);


            return data;
        }

        /// <summary>
        /// Creates a IDiscordClient logged in as the user. Not very useful though... lacks pretty much all permissions. Must be disposed after use.
        /// </summary>
        /// <returns></returns>
        public async Task<IDiscordClient> GetClient()
        {
            var discordRestClient = new DiscordRestClient(new DiscordSocketConfig());
            await discordRestClient.LoginAsync(Discord.TokenType.Bearer, await getAccessToken(), true);

            return discordRestClient;
        }
    }
}

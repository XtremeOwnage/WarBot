using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace WarBot.UI.Configuration.Authentication;
public static class AuthenticationConfiguration
{
    public static void ConfigureService(IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = "discord";
        })
        .AddCookie()
        .AddOAuth("discord", options =>
        {
            options.AuthorizationEndpoint = "https://discord.com/api/oauth2/authorize";
            options.CallbackPath = "/signin";
            options.ClientId = BotConfig.DISCORD_ID.ToString();
            options.ClientSecret = BotConfig.DISCORD_SECRET;
            options.Scope.Add("guilds");
            options.Scope.Add("guilds.members.read");
            options.Scope.Add("identify");
            options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
            options.CorrelationCookie.IsEssential = true;
            options.CorrelationCookie.SameSite = SameSiteMode.None;
            options.SaveTokens = true;
            options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.TokenEndpoint = "https://discord.com/api/oauth2/token";
            options.UserInformationEndpoint = "/users/@me";

            options.Events.OnRemoteFailure = (err) =>
            {
                Console.WriteLine(err.Failure.Message);
                return Task.CompletedTask;
            };
            options.Events.OnCreatingTicket = async (ctx) =>
            {
                var userInfoRequest = new HttpRequestMessage(HttpMethod.Get, "https://discord.com/api/users/@me");
                userInfoRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                userInfoRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

                var userInfoResponse = await ctx.Backchannel.SendAsync(userInfoRequest);
                var userInfo = await userInfoResponse.Content.ReadFromJsonAsync<Models.Discord.User>();

                var user = ctx.Principal.Identity as ClaimsIdentity;
                user.AddClaim(new Claim(user.NameClaimType, userInfo.username));
                user.AddClaim(new Claim("access_token", ctx.AccessToken));
            };
        });

        services.AddAuthorization(options =>
        {
            // By default, all incoming requests will be authorized according to the default policy
            options.FallbackPolicy = options.DefaultPolicy;
        });
    }
}

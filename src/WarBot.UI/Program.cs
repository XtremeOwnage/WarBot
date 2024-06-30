using Discord;
using Discord.Rest;
using Discord.WebSocket;
using FluentValidation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;
using WarBot.Data;
using WarBot.DataAccess;

var builder = WebApplication.CreateBuilder(args);


#region Load Global Configuration
BotConfigLoader.LoadConfig(builder.Configuration);
#endregion

builder.Configuration.AddEnvironmentVariables();


builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"/opt/warbot/keys"));

// Add services to the container.
WarBot.UI.Configuration.Authentication.AuthenticationConfiguration.ConfigureService(builder.Services);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.Cookie.Name = "WarBOT";
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.IsEssential = true;
});
builder.Services.AddSingleton<WarBot.UI.Components.DiscordObjectCache>();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<WarDB>(ServiceLifetime.Scoped);
builder.Services.AddLocalization();
builder.Services.AddValidatorsFromAssemblyContaining<WarBot.DataAccess.WebsiteLogic>();
builder.Services.AddSingleton(sp =>
{
    var c = new DiscordRestClient(new DiscordSocketConfig
    {
        AlwaysDownloadUsers = false,
        LogGatewayIntentWarnings = true,
        GatewayIntents = GatewayIntents.None,

        //TotalShards = 5,
        DefaultRetryMode = RetryMode.AlwaysRetry,
    });


    return c;
});
builder.Services.AddSingleton<IDiscordClient>(sp => sp.GetRequiredService<DiscordRestClient>());
builder.Services.AddScoped<WarBot.UI.Components.DiscordAPI>();
builder.Services.AddScoped<WebsiteLogic>();

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(IPAddress.Parse("172.16.0.0"), 12));
});
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.RequestHeaders;
    logging.RequestHeaders.Add("x-forwarded-host");
    logging.RequestHeaders.Add("x-forwarded-proto");
    logging.RequestHeaders.Add("x-forwarded-for");
    logging.RequestHeaders.Add("cookie");
    logging.RequestHeaders.Add("CF-Connecting-IP");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;

});

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(5000);
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseForwardedHeaders();

    app.Use((context, next) =>
    {
        context.Request.Scheme = "https";
        return next();
    });
}

app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None,
    OnAppendCookie = (cookie) =>
    {
        Console.WriteLine("Created cookie " + cookie.CookieName);
    },
    OnDeleteCookie = (cookie) =>
    {
        Console.WriteLine("Deleted cookie " + cookie.CookieName);
    },
    Secure = CookieSecurePolicy.Always,
    CheckConsentNeeded = (ctx) => false,
});


app.UseHttpLogging();
app.UseStaticFiles();


app.UseRouting();

#region Localization Support
var supportedCultures = new[] { "en-US", "es-CL" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
#endregion
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(end =>
{
    end.MapBlazorHub();
    end.MapFallbackToPage("/_Host");
});

var discord = app.Services.GetRequiredService<DiscordRestClient>();

////Login  and start discord api.
await discord.LoginAsync(TokenType.Bot, BotConfig.DISCORD_TOKEN, true);


Console.WriteLine($"Login State: {discord.LoginState}");

app.Run();


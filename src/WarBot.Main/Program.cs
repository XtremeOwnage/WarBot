using Discord.Interactions;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.Extensions.Hosting;
using Serilog;
using WarBot;
using WarBot.Core;
using WarBot.Data;
using WarBot.Modules;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("logs/warbot.log", rollingInterval: RollingInterval.Day)
    .WriteTo.File("logs/warbot.err", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();


#region Load Global Configuration
Log.Information("Loading Configuration.");

BotConfigLoader.LoadConfig();

Log.Information($"Bot Invite URL: {BotConfig.DISCORD_INVITE_URL_FULL}");

#endregion

Log.Information("Building Host.");

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddDbContext<WarDB>(ServiceLifetime.Transient);
        services.AddSingleton(sp =>
        {
            var c = new DiscordShardedClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
                LogGatewayIntentWarnings = true,
                GatewayIntents = GatewayIntents.DirectMessageReactions
                    | GatewayIntents.DirectMessages
                    | GatewayIntents.GuildBans
                    | GatewayIntents.GuildIntegrations
                    | GatewayIntents.GuildMembers
                    //| GatewayIntents.GuildPresences
                    //| GatewayIntents.GuildMessageReactions
                    | GatewayIntents.GuildInvites
                    //| GatewayIntents.GuildMessages
                    | GatewayIntents.Guilds
                    | GatewayIntents.GuildScheduledEvents,
                TotalShards = 1,       //Hard-coded to single shard, since this bot is being re-packaged for individual hosting.
                DefaultRetryMode = RetryMode.AlwaysRetry,
            }); ;


            return c;
        });

        //services.AddSingleton<ConnectionMultiplexer>((sp) =>
        //{

        //    var cfg = new ConfigurationOptions
        //    {
        //        AbortOnConnectFail = true,
        //        ConnectTimeout = TimeSpan.FromSeconds(2).Milliseconds,
        //    };
        //    cfg.EndPoints.Add(BotConfig.REDIS_HOST, BotConfig.REDIS_PORT);
        //    return ConnectionMultiplexer.Connect(cfg);
        //});

        //Configure Hangfire
        services.AddHangfire(config =>
        {
            //string connString = $"{BotConfig.REDIS_HOST}:{BotConfig.REDIS_PORT}";
            //var opts = new Hangfire.Redis.RedisStorageOptions
            //{
            //    Prefix = "Hangfire",
            //};
            //config.UseRedisStorage(connString, opts);
            config.UseMemoryStorage();
        });

        services.AddSingleton<Hangfire.BackgroundJobServerOptions>(sp =>
        {
            return new BackgroundJobServerOptions
            {
                ServerName = "WarBOT"
            };
        });


        services.AddSingleton<IDiscordClient>(sp => sp.GetService<DiscordShardedClient>());
        services.AddScoped<WarBot.Modules.CustomCommandHandler>();
        //Lastly, Add the Warbot service.
        services.AddSingleton<WARBOT>();
        services.AddSingleton<IWarBOT>(sp => sp.GetService<WARBOT>());
        services.AddSingleton<IHostedService>(sp => sp.GetService<WARBOT>());

        //Bind the interaction service to the instance hosted on warbot.
        //services.AddSingleton<InteractionService>(sp => sp.GetService<WARBOT>().Interactionservice);
        services.AddSingleton<InteractionService>(sp =>
        {
            var discord = sp.GetRequiredService<DiscordShardedClient>();
            var svc = new InteractionService(discord, new InteractionServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                AutoServiceScopes = true,
                UseCompiledLambda = true,
            });

            return svc;
        });
    })
    .UseSerilog();

IHost host = builder.Build();


Log.Information("host.RunAsync();");

await host.RunAsync();





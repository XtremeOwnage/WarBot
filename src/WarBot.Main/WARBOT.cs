 using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using WarBot.Core;
using WarBot.Data;
using WarBot.Modules;

namespace WarBot
{
    public partial class WARBOT : IHostedService, IWarBOT
    {
        private Hangfire.BackgroundJobServer jobServer { get; set; }
        public InteractionService Interactionservice { get; set; }
        private DiscordShardedClient client { get; init; }
        private ILogger<WARBOT> log { get; init; }
        private ILogger<IDiscordClient> discordLogger { get; init; }
        private IServiceProvider sp { get; init; }
        private static bool HasLoaded { get; set; }


        public WARBOT(ILogger<WARBOT> logger, ILogger<IDiscordClient> dLogger, DiscordShardedClient discordClient, IServiceProvider sp, InteractionService INSRC)
        {
            log = logger;
            discordLogger = dLogger;
            this.client = discordClient ?? throw new NullReferenceException("discordClient is null");
            this.sp = sp;
            this.Interactionservice = INSRC;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            log.LogInformation("Start() called.");

            #region Create/Migration Database, if required.
            using (var scope = sp.CreateScope())
            {
                WarDB db = sp.GetService<WarDB>();
                db.Migrate();

                //Run discord migrations.
                await WarBot.DataAccess.ApplyMigrations.Execute(db, log);
            }
            #endregion

            //Attach basic events to the bot. The rest of the events will be attached after onReady is called.
            //client.Connected += Client_Connected;
            //client.Ready += Client_Ready;
            client.Log += LogAsync;
            Interactionservice.Log += LogAsync;

            client.ChannelDestroyed += Client_ChannelDestroyed;

            client.ShardReady += Client_ShardReady;
            client.ShardConnected += (cl) =>
            {
                log.LogInformation("Shard Connected");
                return Task.CompletedTask;
            };
            client.ShardDisconnected += (ex, cl) =>
            {
                log.LogError(ex.Message);
                return Task.CompletedTask;
            };

            client.JoinedGuild += Bot_GuildAdded;
            client.LeftGuild += Bot_GuildDeleted;

            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;


            //client.GuildAvailable += Client_GuildAvailable;

            //client.MessageDeleted += Client_MessageDeleted_Poll;
            //client.MessageReceived += Client_MessageReceived;
            //client.SelectMenuExecuted += ComponentActionReceived;
            //client.ButtonExecuted += ComponentActionReceived;
            //client.ReactionAdded += Client_ReactionAdded;
            //client.ReactionRemoved += Client_ReactionRemoved;
            client.RoleDeleted += Client_RoleDeleted;


            client.UserLeft += (guild, user) =>
            {
                log.LogInformation($"User {user.Username} joined guild {guild.Name}");
                return Task.CompletedTask;
            };

            //Process Commands.
            client.SlashCommandExecuted += (arg) => SlashCommandExecuted(arg);
            client.MessageCommandExecuted += (arg) => MessageContextCommandExecuted(arg);
            client.UserCommandExecuted += (arg) => UserContextCommandExecuted(arg);

            //Handle Errors.
            Interactionservice.SlashCommandExecuted += (_, arg2, arg3) => HandleInteractionServiceError(arg2, arg3);
            Interactionservice.ContextCommandExecuted += (_, arg2, arg3) => HandleInteractionServiceError(arg2, arg3);
            Interactionservice.ComponentCommandExecuted += (_, arg2, arg3) => HandleInteractionServiceError(arg2, arg3);

            //Add interaction modules.
            await Interactionservice.AddModulesAsync(typeof(WarBot.Modules.IWarBOT).Assembly, this.sp);

            ////Login  and start discord api.
            await client.LoginAsync(TokenType.Bot, BotConfig.DISCORD_TOKEN, true);

            await client.StartAsync();

            log.LogInformation("Start() completed.");
        }

        private Task Client_ShardReady(DiscordSocketClient arg)
        {
            log.LogInformation($"Shard {arg.ShardId} is ready.");

            lock (this)
            {
                if (!HasLoaded)
                {
                    log.LogInformation("Starting background job server.");
                    var hfstorage = sp.GetService<Hangfire.JobStorage>();
                    var hfcfg = sp.GetService<Hangfire.BackgroundJobServerOptions>();
                    jobServer = new Hangfire.BackgroundJobServer(hfcfg, hfstorage);

                    log.LogInformation("Scheduling background jobs.");
                    ScheduledJobs.ScheduleJobs();
                }
            }

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            log.LogError("Stop() was called.");

            log.LogInformation("Stopping background job server.");
            jobServer.Dispose();

            log.LogInformation("Stopping Discord.");

            //Set status as offline.
            await client.SetStatusAsync(UserStatus.Offline);

            //Shutdown the client.
            await client.StopAsync();

            //And.... gracefully (cough* forcefully) kill the process.
            Environment.Exit(0);
        }

        /// <summary>
        /// Creates a service-provider scope, retreives a database context, builds a GuildLogic class, and provides an action to interact with it.
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="Action"></param>
        /// <returns></returns>
        private async Task CreateConfigScope(SocketGuild guild, Func<GuildLogic, Task> Action
#if DEBUG
        , [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
#endif
            )
        {
            using var scope = sp.CreateScope();
            using var db = scope.ServiceProvider.GetService<WarDB>();
            using var cfg = await GuildLogic.GetOrCreateAsync(client, db, guild
#if DEBUG
            , memberName, sourceFilePath, sourceLineNumber
#endif
                );
            await Action(cfg);

        }

        private Task LogAsync(LogMessage message)
        {
            LogLevel sev = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information,
            };

            discordLogger.Log(sev, exception: message.Exception, message: message.Message);

            return Task.CompletedTask;
        }
    }
}

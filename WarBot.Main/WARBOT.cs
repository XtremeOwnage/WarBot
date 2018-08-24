using DiscordBotsList.Api;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WarBot.Storage;
using Discord.Commands;
using System.Reflection;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using WarBot.Core;
using System.Collections.Concurrent;
using WarBot.Core.Dialogs;
using System.Runtime.CompilerServices;
using Hangfire.MemoryStorage;
using Hangfire;
using WarBot.Core.JobScheduling;
using WarBot.Modules.TypeReaders;

namespace WarBot
{
    public partial class WARBOT : IWARBOT
    {
        #region IWarBot implementation
        IDiscordClient IWARBOT.Client => this.Client;
        int IWARBOT.LoadedCommands => this.commands.Commands.Count();
        int IWARBOT.LoadedModules => this.commands.Modules.Count();
        ILog IWARBOT.Log => this.Log;
        long IWARBOT.MessagesProcessed => this.MessagesProcessed;
        Core.Environment IWARBOT.Environment => this.Config.Environment;
        #endregion

        public long MessagesProcessed = 0;
        public CommandService commands;
        public AuthDiscordBotListApi BotListAPI { get; private set; }
        public DiscordSocketClient Client { get; private set; }
        public Util.Log Log { get; private set; }
        public CancellationTokenSource botRunning { get; private set; } = new CancellationTokenSource();
        public IDblSelfBot botListMe { get; set; }
        //Simple, stupid dependancy injection.
        public IServiceProvider services;
        //Container to keep track of cached guild configs.
        public IGuildConfigRepository GuildRepo { get; }
        public BotConfig Config { get; private set; }
        public IJobScheduler Jobs { get; private set; }
        public TimeSpan jobPollingInterval { get; } = TimeSpan.FromSeconds(15);


        /// <summary>
        /// Just need to keep an instance of the background job server, for background jobs to process.
        /// </summary>
        private BackgroundJobServer jobServer;


        public WARBOT()
        {
            this.Config = BotConfig.Load();
            this.Client = new DiscordSocketClient();
            this.commands = new CommandService();
            this.BotListAPI = new AuthDiscordBotListApi(Config.BotId, Config.BotList_API_Token);
            this.Log = new Util.Log(this);
            this.GuildRepo = new GuildConfigRepository();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ShouldHandleMessage(IGuildConfig Cfg)
                    => Cfg != null && Cfg.Environment == this.Config.Environment;

        public async Task Start()
        {
            using (WarDB db = new WarDB(new Microsoft.EntityFrameworkCore.DbContextOptions<WarDB>()))
            {
                await db.Migrate();
            }

            #region Simple, Stupid DI Solution
            //Initialize simple DI solution.
            var sc = new ServiceCollection();
            sc.AddSingleton(this); //add WARBOT.
            sc.AddSingleton<IDiscordClient>(Client);
            sc.AddSingleton<ILog>(Log);
            sc.AddSingleton(GuildRepo);
            sc.AddSingleton<IWARBOT>(this);
            sc.AddDbContext<WarDB>(ServiceLifetime.Singleton);

            sc.AddSingleton(o => new Modules.CommandModules.RemindMeStandAloneJob(this));
            services = sc.BuildServiceProvider();

            #endregion
            #region Background Job Processing
            //Initialize Hangfire (Background Job Server)
            //ToDo - Replace this with a stateful MySql database, if available.
            GlobalConfiguration.Configuration.UseMemoryStorage();

            BackgroundJobServerOptions options = new BackgroundJobServerOptions()
            {
                Activator = new Implementation.HangfireActivator(this.services),
                SchedulePollingInterval = jobPollingInterval,
            };

            this.jobServer = new BackgroundJobServer(options);

            //Hangfire uses a lot of static methods, so, we just have to create the placeholder task.
            this.Jobs = new Implementation.HangfireJobScheduler();
            #endregion

            //Initialize the config repository with an instance of the WarDB from the DI container.
            ((GuildConfigRepository)this.GuildRepo).Initialize(this, services.GetService<WarDB>());

            //Add custom type readers.
            commands.AddTypeReader<TimeSpanext>(new ImprovedTimeSpanTypeReader());

            //Initialize the commands.
            await commands.AddModulesAsync(typeof(Modules.Dialogs.MimicMeDialog).Assembly);




            //Attach basic events to the bot. The rest of the events will be attached after onReady is called.
            Client.ChannelDestroyed += Client_ChannelDestroyed;
            Client.Connected += Client_Connected;
            Client.Disconnected += Client_Disconnected;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.Log += Client_Log;
            Client.MessageReceived += Client_MessageReceived;
            Client.Ready += Client_Ready;
            Client.RoleDeleted += Client_RoleDeleted;
            Client.UserJoined += Client_UserJoined;
            Client.UserLeft += Client_UserLeft;

            //Open the bot list API.
            botListMe = await BotListAPI.GetMeAsync();

            ////Login  and start discord api.
            await Client.LoginAsync(TokenType.Bot, Config.Discord_API_Token, true);
            await Client.StartAsync();
        }

        private async Task Client_Disconnected(Exception arg)
        {
            await Log.Error(null, arg);
        }

        private async Task Client_Connected()
        {
            await Log.Debug("Client Connected.");
        }

        private async Task Client_Ready()
        {
            await UpdateBotStats();

            //Set status to online.
            await Client.SetStatusAsync(UserStatus.Online);
            await Log.Debug("Discord Ready");
        }

        private async Task UpdateBotStats()
        {
            await botListMe.UpdateStatsAsync(Client.Guilds.Count);
        }
    }
}

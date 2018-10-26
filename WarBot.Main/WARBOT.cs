﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Ninject;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.JobScheduling;
using WarBot.Storage;

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
        CommandService IWARBOT.CommandService => this.commands;
        #endregion

        public long MessagesProcessed = 0;
        public CommandService commands;
        public DiscordSocketClient Client { get; private set; }
        public Util.Log Log { get; private set; }
        public CancellationTokenSource StopToken { get; private set; } = new CancellationTokenSource();

        //Kernel is static- because I have not yet implemented a method for sharing it to the Quartz scheduler.
        public static IKernel kernel { get; private set; }
        public GuildConfigRepository GuildRepo { get; }
        public BotConfig Config { get; private set; }
        public IJobScheduler Jobs { get; private set; }
        public TimeSpan jobPollingInterval { get; } = TimeSpan.FromSeconds(2);

        IGuildConfigRepository IWARBOT.GuildRepo => this.GuildRepo;


        public WARBOT()
        {
            this.Config = BotConfig.Load();
            this.Client = new DiscordSocketClient();
            this.commands = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                CaseSensitiveCommands = false,
                IgnoreExtraArgs = true,
            });
            this.Log = new Util.Log(this);
            this.GuildRepo = new GuildConfigRepository();
        }

        public void Start()
        {
            #region Simple, Stupid DI Solution
            //Initialize simple DI solution.
            kernel = new StandardKernel();
            kernel.Bind<IWARBOT, WARBOT>().ToConstant(this);
            kernel.Bind<IDiscordClient>().ToConstant(this.Client);
            kernel.Bind<ILog>().ToConstant(this.Log);
            kernel.Bind<IGuildConfigRepository>().ToConstant(this.GuildRepo);
            kernel.Bind<WarDB>().ToSelf().InThreadScope();
            #endregion

            #region Create/Migration Database, if required.
            var db = kernel.Get<WarDB>();

            db.Migrate();
            #endregion

            Quartz.Logging.LogProvider.SetCurrentLogProvider(new Util.QuartzLogProvider());
            this.Jobs = new Implementation.QuartzJobScheduler(this);

            //Initialize the config repository with an instance of the WarDB from the DI container.
            this.GuildRepo.Initialize(this, db);


            //Initialize the commands.
            var discord_1 = commands.AddModulesAsync(typeof(Modules.Dialogs.MimicMeDialog).Assembly, kernel).Result;

            //Load the schedules to execute the war notifications.
            ScheduledJobs.ScheduleJobs(this.Jobs);


            //Attach basic events to the bot. The rest of the events will be attached after onReady is called.
            Client.ChannelDestroyed += Client_ChannelDestroyed;
            Client.Disconnected += Client_Disconnected;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.JoinedGuild += Client_JoinedGuild;
            Client.LeftGuild += Client_LeftGuild;
            Client.Log += Client_Log;
            Client.MessageDeleted += Client_MessageDeleted_Poll;
            Client.MessageReceived += Client_MessageReceived;
            Client.ReactionAdded += Client_ReactionAdded;
            Client.ReactionRemoved += Client_ReactionRemoved;
            Client.Ready += Client_Ready;
            Client.RoleDeleted += Client_RoleDeleted;
            Client.UserJoined += Client_UserJoined;
            Client.UserLeft += Client_UserLeft;

            ////Login  and start discord api.
            Client.LoginAsync(TokenType.Bot, Config.Discord_API_Token, true).Wait();
            Client.StartAsync().Wait();
        }

        private async Task Client_Disconnected(Exception arg)
        {
            await Console.Out.WriteLineAsync(arg.Message);
            try
            {
                while (true)
                {
                    switch (Client.ConnectionState)
                    {
                        case Discord.ConnectionState.Connected:
                            //Good to go.
                            return;
                        case Discord.ConnectionState.Disconnecting:
                        case Discord.ConnectionState.Connecting:
                            //Try again, in 4 seconds.
                            await Task.Delay(TimeSpan.FromSeconds(4), StopToken.Token);
                            return;
                        case Discord.ConnectionState.Disconnected:
                            StopToken.Cancel();
                            return;


                    }
                }
            }
            catch (Exception ex)
            {
                //Write exception to console, force bot to stop.
                await Console.Out.WriteLineAsync(ex.Message);
                StopToken.Cancel();
            }
        }

        private Task Client_Ready()
        {
            //Set status to online.
            return Client.SetStatusAsync(UserStatus.Online);
        }
    }
}

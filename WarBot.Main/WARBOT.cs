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
        public IGuildConfigRepository GuildRepo;
        public BotConfig Config { get; private set; }

        public WARBOT()
        {
            this.Config = BotConfig.Load();
            this.Client = new DiscordSocketClient();
            this.commands = new CommandService();
            this.BotListAPI = new AuthDiscordBotListApi(Config.BotId, Config.BotList_API_Token);
            this.Log = new Util.Log(this);
            this.GuildRepo = new GuildConfigRepository();
        }

        //ToDo - This is scoped so it will only listen to events from the TEST guild.
        private bool ShouldHandleMessage(IGuildConfig Cfg)
            => Cfg.Guild.Id == 458992709718245377 || Cfg.Guild.Id == 461975072563789824;
        //    => this.Environment == Cfg.Environment && Cfg.Guild.ID == 458992709718245377;

        public async Task Start()
        {
            using (WarDB db = new WarDB(new Microsoft.EntityFrameworkCore.DbContextOptions<WarDB>()))
            {
                await db.Migrate();
            }
            //Initialize simple DI solution.
            var sc = new ServiceCollection();
            sc.AddSingleton(this); //add WARBOT.
            sc.AddSingleton<IDiscordClient>(Client);
            sc.AddSingleton<ILog>(Log);
            sc.AddSingleton<IGuildConfigRepository>(GuildRepo);
            sc.AddSingleton<IWARBOT>(this);
            sc.AddDbContext<WarDB>(ServiceLifetime.Singleton);
            services = sc.BuildServiceProvider();

            //Initialize the config repository with an instance of the WarDB from the DI container.
            ((GuildConfigRepository)this.GuildRepo).Initialize(this, services.GetService<WarDB>());

            //Initialize the commands.
            await commands.AddModulesAsync(typeof(Modules.StatsModule).Assembly);


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

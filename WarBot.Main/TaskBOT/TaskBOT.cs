using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.TaskBOT
{
    public partial class TaskBOT : ITaskBOT
    {
        public WARBOT BOT { get; }
        public DiscordSocketClient Client { get; }

        public Util.Log Log => BOT.Log;

        public TaskBOT(WARBOT bot)
        {
            BOT = bot;
            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                AlwaysDownloadUsers = false,
            });
        }

        public void Start(Util.Log Log)
        {
            Log.Debug("Start TaskBot() called.").Wait();

            ////Login  and start discord api.
            Client.LoginAsync(TokenType.Bot, BOT.Config.Discord_API_Token, true).Wait();
            Client.Ready += Client_Ready;
            Client.StartAsync().Wait();
        }

        private async Task Client_Ready()
        {
            await Log.Debug("TaskBOT Ready.");
        }

        private async Task Client_Disconnected(Exception arg)
        {
            await Console.Out.WriteLineAsync(arg.Message);

            //StopToken.Cancel();
        }
    }
}

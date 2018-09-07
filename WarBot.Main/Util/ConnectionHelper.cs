using System;
using System.Collections.Generic;
using System.Text;
using WarBot.Core;

namespace WarBot.Util
{
    public class ConnectionHelper
    {
        private IWARBOT bot { get; }

        public ConnectionHelper(IWARBOT BOT)
        {
            bot = BOT;
        }

        public void EnsureConnectedOrExit()
        {
            switch (bot.Client.ConnectionState)
            {
                case Discord.ConnectionState.Connected:
                    //Good to go.
                    return;
                case Discord.ConnectionState.Disconnecting:
                case Discord.ConnectionState.Connecting:
                    //Try again, in 4 seconds.
                    bot.Jobs.Schedule<ConnectionHelper>(o => o.EnsureConnectedOrExit(), TimeSpan.FromSeconds(4));
                    return;
                case Discord.ConnectionState.Disconnected:
                    var b = bot as WARBOT;
                    //Force the bot to stop.... gracefully.
                    b.botRunning.Cancel();
                    return;


            }
        }
    }
}

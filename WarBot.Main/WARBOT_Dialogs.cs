using DiscordBotsList.Api;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WarBot.Storage;
using Discord.Commands;
using System.Threading;
using WarBot.Core.Dialogs;
using System.Collections.Concurrent;

namespace WarBot
{
    public partial class WARBOT
    {
        /// <summary>
        /// Dialogs, will force a specific user/channel combination, into a "stateful" conversation with the bot.
        /// Examples: A bot setup dialog, Data entry, etc.
        /// While a user is inside of a dialog, other message handlers will be bypassed.
        /// </summary>


        //Keep track of current, open dialogs.
        public ConcurrentDictionary<int, SocketGuildDialogContextBase> Dialogs = new ConcurrentDictionary<int, SocketGuildDialogContextBase>();

        public void OpenDialog(SocketGuildDialogContextBase Dialog)
        {
            this.Dialogs.TryAdd(Dialog.GetHashCode(), Dialog);
        }
        public void CloseDialog(SocketGuildDialogContextBase Dialog)
        {
            this.Dialogs.TryRemove(Dialog.GetHashCode(), out var _);
        }
    }
}

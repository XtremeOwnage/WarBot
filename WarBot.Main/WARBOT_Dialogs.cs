using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;

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
        public ConcurrentDictionary<int, SocketDialogContextBase> Dialogs = new ConcurrentDictionary<int, SocketDialogContextBase>();

        public async Task OpenDialog(SocketDialogContextBase Dialog)
        {
            await Dialog.OnCreated();

            this.Dialogs.TryAdd(Dialog.GetHashCode(), Dialog);
        }
        public async Task CloseDialog(SocketDialogContextBase Dialog)
        {
            try
            {
                await Dialog.OnClosed();
            }
            catch (Exception ex)
            {
                await this.Log.Error(null, ex);
            }

            this.Dialogs.TryRemove(Dialog.GetHashCode(), out var _);
        }
    }
}

//using System.Collections.Concurrent;
//using WarBot.Modules;
//using WarBot.Modules.Base;

//namespace WarBot
//{
//    public partial class WARBOT : IWarBOT
//    {
//        /// <summary>
//        /// Dialogs, will force a specific user/channel combination, into a "stateful" conversation with the bot.
//        /// Examples: A bot setup dialog, Data entry, etc.
//        /// While a user is inside of a dialog, other message handlers will be bypassed.
//        /// </summary>


//        //Keep track of current, open dialogs.
//        public ConcurrentDictionary<int, SocketDialogContextBase> Dialogs = new ConcurrentDictionary<int, SocketDialogContextBase>();

//        public bool TryGetDialog<T>(ISocketMessageChannel Channel, IUser User, out T Dialog) where T : SocketDialogContextBase
//        {
//            var hc = (User.Id + Channel.Id).GetHashCode();

//            if (Dialogs.TryGetValue(hc, out var dialog) && dialog is T u)
//            {
//                Dialog = u;
//                return true;
//            }
//            Dialog = default;
//            return false;
//        }


//        public async Task OpenDialog(SocketDialogContextBase Dialog)
//        {
//            await Dialog.OnCreated();

//            this.Dialogs.TryAdd(Dialog.GetHashCode(), Dialog);
//        }
//        public async Task CloseDialog(SocketDialogContextBase Dialog)
//        {
//            try
//            {
//                await Dialog.OnClosed();
//            }
//            catch (Exception ex)
//            {
//                this.log.LogError(ex.Message);
//            }

//            this.Dialogs.TryRemove(Dialog.GetHashCode(), out var _);
//        }
//    }
//}

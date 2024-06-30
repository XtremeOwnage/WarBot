using WarBot.Core.Voting;

namespace WarBot.Modules;
public interface IWarBOT
{
    /// <summary>
    /// Determines if a dialog is currently open.
    /// </summary>
    /// <param name="Channel"></param>
    /// <param name="User"></param>
    /// <returns></returns>
    //bool TryGetDialog<T>(ISocketMessageChannel Channel, IUser User, out T Dialog) where T : SocketDialogContextBase;
    /// <summary>
    /// Open a new dialog with the user/channel combination.
    /// </summary>
    /// <param name="Dialog"></param>
    /// <returns></returns>
    //Task OpenDialog(SocketDialogContextBase Dialog);

    /// <summary>
    /// Closes a dialog.
    /// </summary>
    /// <param name="Dialog"></param>
    /// <returns></returns>
    //Task CloseDialog(SocketDialogContextBase Dialog);

    /// <summary>
    /// Adds a poll to the bot.
    /// </summary>
    /// <param name="Poll"></param>
    void AddPoll(Poll Poll, TimeSpan WhenDone);
}

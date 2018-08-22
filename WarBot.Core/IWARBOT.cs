using Discord;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.JobScheduling;

namespace WarBot.Core
{
    public interface IWARBOT
    {
        /// <summary>
        /// The discord client.
        /// </summary>
        IDiscordClient Client { get; }

        /// <summary>
        /// Reference to the logging system.
        /// </summary>
        ILog Log { get; }

        /// <summary>
        /// The current "Environment" of this process.
        /// </summary>
        Environment Environment { get; }

        /// <summary>
        /// Responsible for managing scheduled jobs and tasks.
        /// </summary>
        IJobScheduler Jobs { get; }



        int LoadedModules { get; }
        int LoadedCommands { get; }
        long MessagesProcessed { get; }

        /// <summary>
        /// Open a new dialog with the user/channel combination.
        /// </summary>
        /// <param name="Dialog"></param>
        /// <returns></returns>
        Task OpenDialog(SocketGuildDialogContextBase Dialog);

        /// <summary>
        /// Closes a dialog.
        /// </summary>
        /// <param name="Dialog"></param>
        /// <returns></returns>
        Task CloseDialog(SocketGuildDialogContextBase Dialog);
    }
}
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.JobScheduling;
using WarBot.Core.Voting;

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
        /// Responsible for managing scheduled jobs and tasks.
        /// </summary>
        IJobScheduler Jobs { get; }

        /// <summary>
        /// Guild config repository.
        /// </summary>
        IGuildConfigRepository GuildRepo { get; }

        /// <summary>
        /// TaskBOT - For running tasks without blocking the main pipeline, like mass deleting messages.
        /// </summary>
        ITaskBOT TaskBot { get; }

        int LoadedModules { get; }
        int LoadedCommands { get; }
        long MessagesProcessed { get; }
        /// <summary>
        /// Determines if a dialog is currently open.
        /// </summary>
        /// <param name="Channel"></param>
        /// <param name="User"></param>
        /// <returns></returns>
        bool TryGetDialog<T>(ISocketMessageChannel Channel, IUser User, out T Dialog) where T : SocketDialogContextBase;
        /// <summary>
        /// Open a new dialog with the user/channel combination.
        /// </summary>
        /// <param name="Dialog"></param>
        /// <returns></returns>
        Task OpenDialog(SocketDialogContextBase Dialog);

        /// <summary>
        /// Closes a dialog.
        /// </summary>
        /// <param name="Dialog"></param>
        /// <returns></returns>
        Task CloseDialog(SocketDialogContextBase Dialog);

        /// <summary>
        /// Exposes the Command service.
        /// </summary>
        CommandService CommandService { get; }

        /// <summary>
        /// Adds a poll to the bot.
        /// </summary>
        /// <param name="Poll"></param>
        void AddPoll(Poll Poll, TimeSpan WhenDone);

        /// <summary>
        /// This token keeps the bot running. If cancellation is requested, entire application will stop.
        /// </summary>
        CancellationTokenSource StopToken { get; }
    }
}
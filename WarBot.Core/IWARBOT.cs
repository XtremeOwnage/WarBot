using Discord;
using Discord.Commands;
using System;
using System.Threading;
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

        /// <summary>
        /// Guild config repository.
        /// </summary>
        IGuildConfigRepository GuildRepo { get; }

        int LoadedModules { get; }
        int LoadedCommands { get; }
        long MessagesProcessed { get; }

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

        ///// <summary>
        ///// Adds a poll to the bot.
        ///// </summary>
        ///// <param name="Poll"></param>
        //Task AddPoll(IPoll Poll, TimeSpan WhenDone);

        /// <summary>
        /// Ends a poll.
        /// </summary>
        /// <param name="MessageId"></param>
        Task EndPoll(ulong MessageId);

        /// <summary>
        /// This token keeps the bot running. If cancellation is requested, entire application will stop.
        /// </summary>
        CancellationTokenSource StopToken { get; }
    }
}
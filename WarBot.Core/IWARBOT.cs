using Discord;
using System.Collections.Concurrent;
using WarBot.Core.Dialogs;

namespace WarBot.Core
{
    public interface IWARBOT
    {
        IDiscordClient Client { get; }
        ILog Log { get; }
        Environment Environment { get; }

        int LoadedModules { get; }
        int LoadedCommands { get; }
        long MessagesProcessed { get; }

        void OpenDialog(SocketGuildDialogContextBase Dialog);
        void CloseDialog(SocketGuildDialogContextBase Dialog);
    }
}
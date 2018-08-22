using Discord;
using System.Threading.Tasks;
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

        Task OpenDialog(SocketGuildDialogContextBase Dialog);
        Task CloseDialog(SocketGuildDialogContextBase Dialog);
    }
}
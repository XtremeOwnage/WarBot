using Discord;

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
    }
}
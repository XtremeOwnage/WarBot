using Discord.Commands;
using System;

namespace WarBot.Core
{
    /// <summary>
    /// Gives a usage example.
    /// Valid replacements:
    /// {command} - Will be replaced with the command name.
    /// {prefix} - Will be replaced with the bot's configured prefix.
    /// </summary>
    public class CommandUsageAttribute : Attribute
    {
        public string Usage { get; }

        public string GetUsage(CommandInfo cmd, IGuildConfig cfg)
        {
            return Usage
                .Replace("{command}", cmd.Name, StringComparison.OrdinalIgnoreCase)
                .Replace("{prefix}", cfg.Prefix, StringComparison.OrdinalIgnoreCase);
        }

        public CommandUsageAttribute(string Usage)
        {
            this.Usage = Usage;
        }
    }
}

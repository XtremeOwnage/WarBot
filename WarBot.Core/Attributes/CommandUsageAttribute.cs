using Discord.Commands;
using System;

namespace WarBot.Core
{
    /// <summary>
    /// Gives a usage example.
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

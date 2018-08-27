using System;

namespace WarBot.Core
{
    /// <summary>
    /// Gives a usage example.
    /// </summary>
    public class CommandUsageAttribute : Attribute
    {
        public string Usage { get; }

        public CommandUsageAttribute(string Usage)
        {
            this.Usage = Usage;
        }
    }
}

using Discord;
using Discord.Commands;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WarBot.Core
{
    public interface ILog
    {
        /// <summary>
        /// Logs a command directed to WarBOT.
        /// </summary>
        Task ChatMessage(IMessage Message, IGuild Guild, IResult Result);
        /// <summary>
        /// Logs a message to the warbot Console.
        /// </summary>
        Task ConsoleOUT(string Message);
        /// <summary>
        /// Logs a message to the warbot debug channel.
        /// </summary>
        Task Debug(string Message, IGuild Guild = null);
        /// <summary>
        /// Logs an exception with details to the warbot error channel.
        /// </summary>
        Task Error(IGuild guild, Exception ex, [CallerMemberName] string Method = "");
        /// <summary>
        /// Send's a message to the discord guild's leadership.
        /// </summary>
        /// <returns>True if a message was successfully delivered.</returns>
        Task<bool> MessageServerLeadership(IGuildConfig cfg, string ErrorMessage);

        /// <summary>
        /// Log when a guild's config is updated to a newer version.
        /// </summary>
        /// <param name="GuildName">The guild's name.</param>
        /// <param name="newVersion">The new version.</param>
        /// <param name="UpdateSentToClan">If an update message was sent to the clan's update channel.</param>
        /// <returns></returns>
        Task GuildUpdated(string GuildName, double oldVersion, double newVersion, bool UpdateSentToClan);
    }
}
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WarBot.Core
{
    public interface IGuildConfig
    {
        /// <summary>
        /// The prefix to which warbot will respond to.
        /// </summary>
        string Prefix { get; set; }
        string Website { get; set; }
        string Loot { get; set; }
        /// <summary>
        /// Latest version of the BOT this guild was active for. Used for sending update notifications.
        /// </summary>
        string BotVersion { get; set; }

        /// <summary>
        /// The guild, this config belongs to.
        /// This value is mainly, to make the config files readable.
        /// </summary>
        SocketGuild Guild { get; }
        /// <summary>
        /// The current guild user for WarBot.
        /// </summary>
        SocketGuildUser CurrentUser { get; }

        /// <summary>
        /// Common notification settings.
        /// </summary>
        INotificationSettings Notifications { get; }

        ITextChannel GetGuildChannel(WarBotChannelType role);
        void SetGuildChannel(WarBotChannelType role, ITextChannel Channel);

        IRole GetGuildRole(RoleLevel role);
        void SetGuildRole(RoleLevel role, IRole GuildRole);
        void ClearAllRoles();
        /// <summary>
        /// Return a mapping of ChannelTypes to Channel instances.
        /// </summary>
        /// <returns></returns>
        IDictionary<WarBotChannelType, ITextChannel> GetChannelMap();
        /// <summary>
        /// Returns a mapping of WarBot role, to discord guild roles for this guild.
        /// </summary>
        /// <returns></returns>
        IDictionary<RoleLevel, IRole> GetRoleMap();
        /// <summary>
        /// This initializes the config.
        /// Loads all of the references, sets the save method... etc.
        /// Must be called before a config can be utilized!!
        /// </summary>
        void Initialize(SocketGuild Guild, Func<IGuildConfig, Task> SaveFunc);
        /// <summary>
        /// Save this config to disk.
        /// </summary>
        /// <returns></returns>
        Task SaveConfig();
        /// <summary>
        /// This will set all settings to default settings.
        /// </summary>
        /// <returns></returns>
        Task SetDefaults(SocketGuild Client);
    }
}
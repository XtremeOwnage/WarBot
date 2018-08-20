using Discord;
using System;
using System.Threading.Tasks;

namespace WarBot.Core
{
    public interface IGuildConfig
    {
        string Website { get; set; }
        string Loot { get; set; }
        /// <summary>
        /// Latest version of the BOT this guild was active for. Used for sending update notifications.
        /// </summary>
        string BotVersion { get; set; }

        /// <summary>
        /// Which channel to send Member-related messages to.
        /// </summary>
        IEntityStorage<ITextChannel> Channel_WAR_Notifications { get; set; }
        IEntityStorage<ITextChannel> Channel_NewUser_Welcome { get; set; }
        IEntityStorage<ITextChannel> Channel_Officers { get; set; }
        IEntityStorage<ITextChannel> Channel_WarBot_News { get; set; }

        /// <summary>
        /// Which environment is this bot? Used to keep prod/nonprod/other instances seperate.
        /// </summary>
        Environment Environment { get; set; }
        /// <summary>
        /// The guild, this config belongs to.
        /// This value is mainly, to make the config files readable.
        /// </summary>
        IEntityStorage<IGuild> Guild { get; }
        /// <summary>
        /// This is the nickname the bot will assume when it joins.
        /// </summary>
        string NickName { get; set; }
        /// <summary>
        /// Common notification settings.
        /// </summary>
        INotificationSettings Notifications { get; }
        IEntityStorage<IRole> Role_Admin { get; set; }
        IEntityStorage<IRole> Role_Leader { get; set; }
        IEntityStorage<IRole> Role_Member { get; set; }
        IEntityStorage<IRole> Role_Officer { get; set; }

        /// <summary>
        /// This initializes the config.
        /// Loads all of the references, sets the save method... etc.
        /// Must be called before a config can be utilized!!
        /// </summary>
        Task Initialize(IDiscordClient Client, IGuild Guild, Func<IGuildConfig, Task> SaveFunc);
        /// <summary>
        /// Save this config to disk.
        /// </summary>
        /// <returns></returns>
        Task SaveConfig();
        /// <summary>
        /// This will set all settings to default settings.
        /// </summary>
        /// <returns></returns>
        Task SetDefaults(IDiscordClient Client);
    }
}
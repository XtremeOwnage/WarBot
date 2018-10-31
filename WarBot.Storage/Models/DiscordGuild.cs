using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Storage.Models.HustleCastle;

namespace WarBot.Storage.Models
{
    /// <summary>
    /// Represent a guild in discord.
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class DiscordGuild : BaseDiscordEntity, IGuildConfig
    {
        public DiscordGuild() { }
        public static DiscordGuild Create(SocketGuild Guild)
        {
            return new DiscordGuild
            {
                EntityId = Guild.Id,
                Name = Guild.Name,
                Value = Guild,
                BotVersion = "2.5",
                NotificationSettings = GuildNotificationsSettings.CreateNew(),
                WarBOT_Prefix = "bot,"
            };
        }

        /// <summary>
        /// Holds a reference to the discord Guild Entity, IF, Initialize has been called.
        /// </summary>
        [NotMapped]
        public SocketGuild Value { get; set; }

        /// <summary>
        /// Reference to this clan's hustle castle guild.
        /// </summary>
        public virtual HustleGuild HustleClan { get; set; }

        /// <summary>
        /// This function will save the guild and its configuration.
        /// </summary>
        [NotMapped]
        private Func<IGuildConfig, Task> saveFunc;

        /// <summary>
        /// The prefix to which warbot will respond to.
        /// </summary>
        public string WarBOT_Prefix { get; set; }

        /// <summary>
        /// Common notification settings.
        /// </summary>
        public virtual GuildNotificationsSettings NotificationSettings { get; private set; }

        /// <summary>
        /// The last version of the bot, this guild was utilizing. Used to send update notifications.
        /// </summary>
        public string BotVersion { get; set; }

        /// <summary>
        /// What message to display when somebody request's the website.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// A random message stored, for loot direction.
        /// </summary>
        public string Loot { get; set; }


        /// <summary>
        /// These are the Discord/Guild-level admins of the bot.
        /// </summary>
        public virtual List<GuildRole> Roles { get; } = new List<GuildRole>();

        /// <summary>
        /// The channels configured for this guild.
        /// </summary>
        public virtual List<GuildChannel> Channels { get; } = new List<GuildChannel>();

        #region Channels and Roles
        //Clear's all configured roles.
        public void ClearAllRoles()
        {
            this.Roles.Clear();
        }
        public IDictionary<RoleLevel, IRole> GetRoleMap()
        {
            return this.Roles
                .Where(o => o.Value != null)
                .ToDictionary(o => o.Level, o => o.Value);
        }
        public IRole GetGuildRole(RoleLevel level) => this.Roles.FirstOrDefault(o => o.Level == level)?.Value;
        public void SetGuildRole(RoleLevel level, IRole GuildRole)
        {
            //Remove the specified role type.
            if (GuildRole == null)
            {
                this.Roles.RemoveAll(o => o.Level == level);
                return;
            }

            if (this.Roles.FirstOrDefault(o => o.Level == level).IsNotNull(out var r))
                r.Set(GuildRole);
            else
                this.Roles.Add(new Models.GuildRole(level, GuildRole));
        }

        public IDictionary<WarBotChannelType, ITextChannel> GetChannelMap()
        {
            return this.Channels
                .Where(o => o.Value != null)
                .ToDictionary(o => o.ChannelType, o => o.Value);
        }
        public ITextChannel GetGuildChannel(WarBotChannelType ChannelType) => this.Channels.FirstOrDefault(o => o.ChannelType == ChannelType)?.Value;
        public void SetGuildChannel(WarBotChannelType ChannelType, ITextChannel Channel)
        {
            //Remove the specified channel type.
            if (Channel == null)
            {
                this.Channels.RemoveAll(o => o.ChannelType == ChannelType);
                return;
            }

            if (this.Channels.FirstOrDefault(o => o.ChannelType == ChannelType).IsNotNull(out var r))
                r.Set(Channel);
            else
                this.Channels.Add(new Models.GuildChannel(ChannelType, Channel));
        }
        #endregion
        #region IGuildConfig Implementation
        [NotMapped]
        public ILog Log { get; private set; }

        string IGuildConfig.Website
        {
            get => this.Website;
            set => this.Website = value;
        }
        string IGuildConfig.Prefix
        {
            get => string.IsNullOrEmpty(WarBOT_Prefix) ? "bot," : WarBOT_Prefix;
            set => this.WarBOT_Prefix = value;
        }
        string IGuildConfig.Loot
        {
            get => this.Loot;
            set => this.Loot = value;
        }
        string IGuildConfig.BotVersion
        {
            get => this.BotVersion;
            set => this.BotVersion = value;
        }

        SocketGuild IGuildConfig.Guild => this.Value;
        SocketGuildUser IGuildConfig.CurrentUser => this.Value.CurrentUser;

        INotificationSettings IGuildConfig.Notifications => this.NotificationSettings;



        public void Initialize(SocketGuild Guild, Func<IGuildConfig, Task> SaveFunc, IWARBOT Bot)
        {
            this.Log = Bot.Log;

            //Initialize the stored references.
            foreach (var role in this.Roles.ToList())
            {
                var dRole = Guild.GetRole(role.EntityId);
                if (dRole != null)
                    role.Set(dRole);
                //If the value is null, remove it from the config.
                else
                    this.Roles.Remove(role);

            }

            foreach (var channel in this.Channels.ToList())
            {
                var ch = Guild.GetChannel(channel.EntityId) as ITextChannel;
                if (ch != null)
                    channel.Set(ch);
                //If the value is null, remove it from the config.
                else
                    this.Channels.Remove(channel);

            }

            this.Value = Guild;
            this.saveFunc = SaveFunc;
        }

        public async Task SaveConfig() => await this.saveFunc.Invoke(this);


        public Task SetDefaults(SocketGuild Guild)
        {

            this.Value = Guild;
            this.Name = Guild.Name;

            //Clear all configured roles, and channels.
            this.ClearAllRoles();
            this.Channels.Clear();

            NotificationSettings.setDefaults();

            return Task.CompletedTask;
        }
        #endregion

    }
}

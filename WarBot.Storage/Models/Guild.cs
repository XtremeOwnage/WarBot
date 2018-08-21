using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Storage.Models
{
    [DebuggerDisplay("Name = {Name}")]
    public class Guild : IGuildConfig
    {
        public Guild() { }
        public static Guild Create(SocketGuild Guild)
        {
            return new Guild
            {
                GuildId = Guild.Id,
                Name = Guild.Name,
                Value = Guild,
                Config = GuildConfig.New(),
            };
        }

        #region Discord IGUILD Entity
        [Key]
        public ulong GuildId { get; private set; }

        public string Name { get; set; }

        [NotMapped]
        public SocketGuild Value { get; set; }
        #endregion

        [ForeignKey("ConfigId")]
        public virtual GuildConfig Config { get; set; }

        /// <summary>
        /// This function will save the guild and its configuration.
        /// </summary>
        [NotMapped]
        private Func<IGuildConfig, Task> saveFunc;

        #region Channels and Roles
        public IDictionary<RoleLevel, IRole> GetRoleMap()
        {
            return this.Config.Roles
                .Where(o => o.Value != null)
                .ToDictionary(o => o.Level, o => o.Value);
        }
        public IRole GetGuildRole(RoleLevel level) => this.Config.Roles.FirstOrDefault(o => o.Level == level)?.Value;
        public void SetGuildRole(RoleLevel level, IRole GuildRole)
        {
            //Remove the specified channel type.
            if (GuildRole == null)
            {
                if (this.Config.Roles.FirstOrDefault(o => o.Level == level).IsNotNull(out var deleteMe))
                {
                    this.Config.Roles.Remove(deleteMe);
                }
                return;
            }

            if (this.Config.Roles.FirstOrDefault(o => o.Level == level).IsNotNull(out var r))
                r.Set(GuildRole);
            else
                this.Config.Roles.Add(new Models.GuildRole(level, GuildRole));
        }

        public IDictionary<WarBotChannelType, ITextChannel> GetChannelMap()
        {
            return this.Config.Channels
                .Where(o => o.Value != null)
                .ToDictionary(o => o.ChannelType, o => o.Value);
        }
        public ITextChannel GetGuildChannel(WarBotChannelType ChannelType) => this.Config.Channels.FirstOrDefault(o => o.ChannelType == ChannelType)?.Value;
        public void SetGuildChannel(WarBotChannelType ChannelType, ITextChannel Channel)
        {
            //Remove the specified channel type.
            if (Channel == null)
            {
                if (this.Config.Channels.FirstOrDefault(o => o.ChannelType == ChannelType).IsNotNull(out var deleteMe))
                {
                    this.Config.Channels.Remove(deleteMe);
                }
                return;
            }

            if (this.Config.Channels.FirstOrDefault(o => o.ChannelType == ChannelType).IsNotNull(out var r))
                r.Set(Channel);
            else
                this.Config.Channels.Add(new Models.GuildChannel(ChannelType, Channel));
        }
        #endregion
        #region IGuildConfig Implementation
        string IGuildConfig.Website
        {
            get => this.Config.Website;
            set => this.Config.Website = value;
        }
        string IGuildConfig.Loot
        {
            get => this.Config.Loot;
            set => this.Config.Loot = value;
        }
        string IGuildConfig.BotVersion
        {
            get => this.Config.BotVersion;
            set => this.Config.BotVersion = value;
        }

        Core.Environment IGuildConfig.Environment
        {
            get => this.Config.Environment;
            set => this.Config.Environment = value;
        }

        SocketGuild IGuildConfig.Guild
        {
            get => this.Value;
        }

        SocketGuildUser IGuildConfig.CurrentUser => this.Value.CurrentUser;
        string IGuildConfig.NickName
        {
            get => this.Config.NickName;
            set => this.Config.NickName = value;
        }

        INotificationSettings IGuildConfig.Notifications
        {
            get => this.Config.NotificationSettings;
        }


        public void Initialize(SocketGuild Guild, Func<IGuildConfig, Task> SaveFunc)
        {
            //Initialize the config object.
            this.Config.Initialize(Guild);

            this.Value = Guild;
            this.saveFunc = SaveFunc;            
        }

        public async Task SaveConfig()
        {
            await this.saveFunc.Invoke(this);
        }

        public async Task SetDefaults(SocketGuild Guild)
        {

            this.Value = Guild;
            this.Name = Guild.Name;

            var firtAdminRole = Guild.Roles
                .OrderByDescending(o => o.Position)
                .Where(o => o.Permissions.Administrator == true)
                .FirstOrDefault();

            var defaultChannel = await ChannelHelper
                .findDefaultChannel(Guild);

            var adminChannel = await ChannelHelper
                .findFirstAdminChannel(Guild);

            Config.Environment = Core.Environment.PROD;

            //These two roles should never be set, Just a sanity check to ensure they are empty.
            SetGuildRole(RoleLevel.GlobalAdmin, null);
            SetGuildRole(RoleLevel.None, null);


            SetGuildRole(RoleLevel.ServerAdmin, firtAdminRole);
            SetGuildRole(RoleLevel.Leader, firtAdminRole);

            //We will empty these roles. Up to the server owner to configure.
            SetGuildRole(RoleLevel.Member, null);
            SetGuildRole(RoleLevel.Officer, null);
            SetGuildRole(RoleLevel.Guest, null);
            SetGuildRole(RoleLevel.SuperMember, null);


            SetGuildChannel(WarBotChannelType.CH_New_Users, defaultChannel);
            SetGuildChannel(WarBotChannelType.CH_Officers, adminChannel);
            SetGuildChannel(WarBotChannelType.CH_WarBot_Updates, adminChannel);
            SetGuildChannel(WarBotChannelType.CH_WAR_Announcements, defaultChannel);

            Config.NickName = "WarBOT";
            Config.NotificationSettings.setDefaults();
        }
        #endregion

    }
}

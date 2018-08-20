using Discord;
using System;
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
        public static Guild Create(IGuild Guild)
        {
            return new Guild
            {
                GuildId = Guild.Id,
                Name = Guild.Name,
                Value = Guild,
                Config = GuildConfig.New(),
            };
        }

        [Key]
        public ulong GuildId { get; private set; }

        public string Name { get; set; }

        [NotMapped]
        public IGuild Value { get; set; }

        [ForeignKey("ConfigId")]
        public virtual GuildConfig Config { get; set; }

        [NotMapped]
        private Func<IGuildConfig, Task> saveFunc;

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
        IEntityStorage<ITextChannel> IGuildConfig.Channel_WAR_Notifications
        {
            get => this.Config.Channel_WAR_Notifications.GetEntity<ITextChannel>();
            set => this.Config.Channel_WAR_Notifications.Set(value);
        }
        IEntityStorage<ITextChannel> IGuildConfig.Channel_NewUser_Welcome
        {
            get => this.Config.Channel_Welcome.GetEntity<ITextChannel>();
            set => this.Config.Channel_Welcome.Set(value);
        }
        IEntityStorage<ITextChannel> IGuildConfig.Channel_Officers
        {
            get => this.Config.Channel_Officers.GetEntity<ITextChannel>();
            set => this.Config.Channel_Officers.Set(value);
        }
        IEntityStorage<ITextChannel> IGuildConfig.Channel_WarBot_News
        {
            get => this.Config.Channel_WarBot_News.GetEntity<ITextChannel>();
            set => this.Config.Channel_WarBot_News.Set(value);
        }

        Core.Environment IGuildConfig.Environment
        {
            get => this.Config.Environment;
            set => this.Config.Environment = value;
        }

        IEntityStorage<IGuild> IGuildConfig.Guild
        {
            get => new EntityStorage<IGuild>(this.Name, this.GuildId);
        }

        string IGuildConfig.NickName
        {
            get => this.Config.NickName;
            set => this.Config.NickName = value;
        }

        INotificationSettings IGuildConfig.Notifications
        {
            get => this.Config.NotificationSettings;
        }

        IEntityStorage<IRole> IGuildConfig.Role_Admin
        {
            get => this.Config.Role_Admin.GetEntity<IRole>();
            set => this.Config.Role_Admin.Set(value);
        }
        IEntityStorage<IRole> IGuildConfig.Role_Leader
        {
            get => this.Config.Role_Leader.GetEntity<IRole>();
            set => this.Config.Role_Leader.Set(value);
        }
        IEntityStorage<IRole> IGuildConfig.Role_Member
        {
            get => this.Config.Role_Member.GetEntity<IRole>();
            set => this.Config.Role_Member.Set(value);
        }
        IEntityStorage<IRole> IGuildConfig.Role_Officer
        {
            get => this.Config.Role_Officer.GetEntity<IRole>();
            set => this.Config.Role_Officer.Set(value);
        }


        public async Task Initialize(IDiscordClient Client, IGuild Guild, Func<IGuildConfig, Task> SaveFunc)
        {
            if (this.Config.Role_Admin?.EntityId != null)
                this.Config.Role_Admin.Value = Guild.GetRole(this.Config.Role_Admin.EntityId.Value);

            if (this.Config.Role_Leader?.EntityId != null)
                this.Config.Role_Leader.Value = Guild.GetRole(this.Config.Role_Leader.EntityId.Value);

            if (this.Config.Role_Officer?.EntityId != null)
                this.Config.Role_Officer.Value = Guild.GetRole(this.Config.Role_Officer.EntityId.Value);

            if (this.Config.Role_Member?.EntityId != null)
                this.Config.Role_Member.Value = Guild.GetRole(this.Config.Role_Member.EntityId.Value);

            if (this.Config.Channel_Welcome?.EntityId != null)
                this.Config.Channel_Welcome.Value = await Guild.GetChannelAsync(this.Config.Channel_Welcome.EntityId.Value);

            if (this.Config.Channel_WAR_Notifications?.EntityId != null)
                this.Config.Channel_WAR_Notifications.Value = await Guild.GetChannelAsync(this.Config.Channel_WAR_Notifications.EntityId.Value);

            if (this.Config.Channel_WarBot_News?.EntityId != null)
                this.Config.Channel_WarBot_News.Value = await Guild.GetChannelAsync(this.Config.Channel_WarBot_News.EntityId.Value);

            this.Value = Guild;

            this.saveFunc = SaveFunc;
        }

        public async Task SaveConfig()
        {
            await this.saveFunc.Invoke(this);
        }

        public async Task SetDefaults(IDiscordClient Client)
        {
            var Guild = await Client.GetGuildAsync(this.GuildId);

            this.Value = Guild;
            this.Name = Guild.Name;

            var firtAdminRole = Guild.Roles
                .OrderByDescending(o => o.Position)
                .Where(o => o.Permissions.Administrator == true)
                .Select(o => o.CreateStorage())
                .FirstOrDefault();

            var defaultChannel = await ChannelHelper
                .findDefaultChannel(Client, Guild)
                .CreateStorage();

            //var adminChannel = await ChannelHelper
            //    .findFirstAdminChannel(Client, Guild)
            //    .CreateStorage();

            //Config.Environment = Core.Environment.PROD;
            //Config.Role_Admin.Set(firtAdminRole);
            //Config.Role_Leader.Set(firtAdminRole);
            //Config.Role_Officer.Set(firtAdminRole);
            //Config.Role_Member.Set(firtAdminRole);
           // Config.Channel_WarBot_News.Set(adminChannel);
            //Config.Channel_Officers.Set(adminChannel);
            //Config.Channel_WAR_Notifications.Set(defaultChannel);
            //Config.Channel_Welcome.Set(defaultChannel);
            //Config.NickName = "WarBOT";
            //Config.NotificationSettings.setDefaults();
        }
        #endregion

    }
}

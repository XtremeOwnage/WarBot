using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models
{
    public class GuildConfig
    {
        public GuildConfig() { }

        public static GuildConfig New()
        {
            return new GuildConfig
            {
                BotVersion = "2.5",
                Channel_Officers = DiscordEntity.CreateNew(),
                Channel_WAR_Notifications = DiscordEntity.CreateNew(),
                Channel_WarBot_News = DiscordEntity.CreateNew(),
                Channel_Welcome = DiscordEntity.CreateNew(),
                Environment = Core.Environment.PROD,
                NickName = "WarBOT",
                NotificationSettings = GuildNotificationsConfig.CreateNew(),
                Role_Admin = DiscordEntity.CreateNew(),
                Role_Leader = DiscordEntity.CreateNew(),
                Role_Member = DiscordEntity.CreateNew(),
                Role_Officer = DiscordEntity.CreateNew(),
            };
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        public string NickName;
        /// <summary>
        /// Which environment does this guild belong to? 
        /// Used to keep PROD guilds running on the prod process, and to allow DEV/Nonprod to run on the non-prod bot.
        /// </summary>
        public Core.Environment Environment { get; set; }

        /// <summary>
        /// Common notification settings.
        /// </summary>
        [ForeignKey("GuildNotificationSettingsId")]
        public virtual GuildNotificationsConfig NotificationSettings { get; private set; }

        /// <summary>
        /// The last version of the bot, this guild was utilizing. Used to send update notifications.
        /// </summary>
        public string BotVersion { get; set; }

        public string Website { get; set; }

        public string Loot { get; set; }

        #region Channels
        /// <summary>
        /// The channel to war notifications to.
        /// </summary>
        [ForeignKey("ch_war_id")]
        public virtual DiscordEntity Channel_WAR_Notifications { get; set; }
        /// <summary>
        /// Channel for new member welcomes.
        /// </summary>
        [ForeignKey("ch_welcome_id")]
        public virtual DiscordEntity Channel_Welcome { get; set; }
        /// <summary>
        /// Channel to send bot updates, and news.
        /// </summary>
        [ForeignKey("ch_news_id")]
        public virtual DiscordEntity Channel_WarBot_News { get; set; }

        /// <summary>
        /// Channel for officers and clan-management.
        /// </summary>
        [ForeignKey("ch_officers_id")]
        public virtual DiscordEntity Channel_Officers { get; set; }
        #endregion

        #region Roles
        /// <summary>
        /// These are the Discord/Guild-level admins of the bot.
        /// </summary>
        [ForeignKey("role_admin_id")]
        public virtual DiscordEntity Role_Admin { get; set; }
        /// <summary>
        /// Clan Leanders
        /// </summary>
        [ForeignKey("role_leader_id")]
        public virtual DiscordEntity Role_Leader { get; set; }

        /// <summary>
        /// Clan Officers
        /// </summary>
        [ForeignKey("role_officer_id")]
        public virtual DiscordEntity Role_Officer { get; set; }
        /// <summary>
        /// Clan Members
        /// </summary>
        [ForeignKey("role_member_id")]
        public virtual DiscordEntity Role_Member { get; set; }

        #endregion
    }
}

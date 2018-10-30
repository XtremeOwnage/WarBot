using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;

namespace WarBot.Storage.Models
{
    /// <summary>
    /// A secondary storage for guild settings.
    /// </summary>
    public class GuildNotificationsSettings : INotificationSettings
    {
        public GuildNotificationsSettings() { }

        public static GuildNotificationsSettings CreateNew()
        {
            return new GuildNotificationsSettings().setDefaults();
        }

        internal GuildNotificationsSettings setDefaults()
        {
            War1Enabled = true;
            War2Enabled = true;
            War3Enabled = true;
            War4Enabled = true;
            WarPrepEnding = true;
            WarPrepEndingMessage = null;
            WarPrepStarted = true;
            WarPrepStartedMessage = null;
            WarStarted = true;
            WarStartedMessage = null;
            SendUpdateMessage = true;
            GreetingMessage = null;
            PortalEnabled = true;
            PortalStartedMessage = null;
            User_Left_Guild = true;
            
            return this;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// This config is always associated with a DiscordGuild.
        /// </summary>
        public virtual DiscordGuild DiscordGuild { get; }


        public bool WarPrepStarted { get; set; } = true;
        public bool WarPrepEnding { get; set; } = true;
        public bool WarStarted { get; set; } = true;
        public bool War1Enabled { get; set; } = true;
        public bool War2Enabled { get; set; } = true;
        public bool War3Enabled { get; set; } = true;
        public bool War4Enabled { get; set; } = true;

        public bool SendUpdateMessage { get; set; } = true;



        public string WarPrepStartedMessage { get; set; } = null;
        public string WarPrepEndingMessage { get; set; } = null;
        public string WarStartedMessage { get; set; } = null;


        public bool PortalEnabled { get; set; } = true;
        public string PortalStartedMessage { get; set; } = null;

        /// <summary>
        /// Welcome message for new players joining the discord guild.
        /// </summary>
        public string GreetingMessage { get; set; } = null;

        public bool User_Left_Guild { get; set; } = false;

        #region INotificationSettings
        string INotificationSettings.NewUserGreeting
        {
            get => string.IsNullOrEmpty(GreetingMessage) ? null : GreetingMessage;
            set => GreetingMessage = value;
        }
        #endregion
    }
}

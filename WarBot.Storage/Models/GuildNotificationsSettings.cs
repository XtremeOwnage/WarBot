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
            return this;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        /// <summary>
        /// This config is always associated with a DiscordGuild.
        /// </summary>
        public virtual DiscordGuild DiscordGuild { get; }


        public bool WarPrepStarted { get; set; }
        public bool WarPrepEnding { get; set; }
        public bool WarStarted { get; set; }
        public bool War1Enabled { get; set; }
        public bool War2Enabled { get; set; }
        public bool War3Enabled { get; set; }
        public bool War4Enabled { get; set; }
        public string WarPrepStartedMessage { get; set; }
        public string WarPrepEndingMessage { get; set; }
        public string WarStartedMessage { get; set; }
        public bool SendUpdateMessage { get; set; }

        /// <summary>
        /// Welcome message for new players joining the discord guild.
        /// </summary>
        public string GreetingMessage { get; set; }


        #region INotificationSettings
        string INotificationSettings.NewUserGreeting
        {
            get => string.IsNullOrEmpty(GreetingMessage) ? null : GreetingMessage;
            set => GreetingMessage = value;
        }
        #endregion
    }
}

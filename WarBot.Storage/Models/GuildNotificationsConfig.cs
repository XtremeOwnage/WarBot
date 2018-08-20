using Discord;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WarBot.Core;

namespace WarBot.Storage.Models
{
    public class GuildNotificationsConfig : INotificationSettings
    {
        public GuildNotificationsConfig() { }

        public static GuildNotificationsConfig CreateNew()
        {
            return new GuildNotificationsConfig().setDefaults();
        }

        internal GuildNotificationsConfig setDefaults()
        {
            War1Enabled = true;
            War2Enabled = true;
            War3Enabled = true;
            War4Enabled = true;
            WarPrepAlmostOver = true;
            WarPrepEndingMessage = null;
            WarPrepStarted = true;
            WarPrepStartedMessage = null;
            WarStarted = true;
            WarStartedMessage = null;
            SendUpdateMessage = true;
            return this;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }


        public bool WarPrepStarted { get; set; }
        public bool WarPrepAlmostOver { get; set; }
        public bool WarStarted { get; set; }
        public bool War1Enabled { get; set; }
        public bool War2Enabled { get; set; }
        public bool War3Enabled { get; set; }
        public bool War4Enabled { get; set; }
        public string WarPrepStartedMessage { get; set; }
        public string WarPrepEndingMessage { get; set; }
        public string WarStartedMessage { get; set; }
        public bool SendUpdateMessage { get; set; }
    }
}

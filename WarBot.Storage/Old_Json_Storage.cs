using Discord;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Storage;

namespace WarBot.Legacy
{
    /// <summary>
    /// Config format version 2.0.
    /// This is the last version which was utilized with the node.js BOT.
    /// </summary>
    public class LegacyGuildConfig
    {
        public LegacyEntityStorage Guild { get; set; }
        public int ConfigVersion { get; set; }
        public LegacyEntityStorage Role_Admin { get; set; }
        public LegacyEntityStorage Role_Leader { get; set; }
        public LegacyEntityStorage Role_Officer { get; set; }
        public LegacyEntityStorage Role_Member { get; set; }
        public LegacyEntityStorage Channel_Member { get; set; }
        public LegacyEntityStorage Channel_Officer { get; set; }
        public string NickName { get; set; }
        public NotificationSettings Notifications { get; set; }
        public string BotVersion { get; set; }
        public Core.Environment? Environment { get; set; }
        public string LootURL { get; set; }
        public string WebsiteURL { get; set; }
    }

    public class LegacyEntityStorage
    {
        public string Name { get; set; }
        public ulong ID { get; set; }
    }

    public class NotificationSettings : INotificationSettings
    {
        public bool WarPrepStarted { get; set; } = true;
        public bool WarPrepAlmostOver { get; set; } = true;
        public bool WarStarted { get; set; } = true;
        public bool War1Enabled { get; set; } = true;
        public bool War2Enabled { get; set; } = true;
        public bool War3Enabled { get; set; } = true;
        public bool War4Enabled { get; set; } = true;
        public string WarPrepStartedMessage { get; set; } = null;
        public string WarPrepEndingMessage { get; set; } = null;
        public string WarStartedMessage { get; set; } = null;
        public bool SendUpdateMessage { get; set; } = true;
    }
}

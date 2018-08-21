using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

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
                Environment = Core.Environment.PROD,
                NickName = "WarBOT",
                NotificationSettings = GuildNotificationsConfig.CreateNew(),
                Roles = new List<GuildRole>(),
                Channels = new List<GuildChannel>()
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


        /// <summary>
        /// These are the Discord/Guild-level admins of the bot.
        /// </summary>
        public virtual List<GuildRole> Roles { get; set; }

        /// <summary>
        /// The channels configured for this guild.
        /// </summary>
        public virtual List<GuildChannel> Channels { get; set; }


        #region Initialize method, used to "inflate" all values
        public void Initialize(SocketGuild Guild)
        {
            foreach (var role in this.Roles)
                if (role.EntityId.HasValue)
                    role.Value = Guild.GetRole(role.EntityId.Value);

            foreach (var channel in this.Channels)
                if (channel.EntityId.HasValue)
                    channel.Value = Guild.GetChannel(channel.EntityId.Value) as ITextChannel;

        }
        #endregion
    }
}

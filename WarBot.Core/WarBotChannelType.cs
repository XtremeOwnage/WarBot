using System.ComponentModel;

namespace WarBot.Core
{
    public enum WarBotChannelType
    {
        [Description("News and updates around WarBOT will be sent to this channel.")]
        WARBOT_UPDATES = 1,
        [Description("WAR Notifications will be sent here. War prep started, war prep ending... etc.")]
        WAR = 2,
        [Description("Functions requring officer/admins will be directed here. Clan management functions... etc.")]
        CH_Officers = 3,
        [Description("If I am configured to welcome new users, I will do it here.")]
        USER_JOIN = 4,
        [Description("If a user leaves the guild, I will send the message to here.")]
        USER_LEFT = 5,
    }
}

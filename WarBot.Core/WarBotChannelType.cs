using System.ComponentModel;

namespace WarBot.Core
{
    public enum WarBotChannelType
    {
        [Description("News and updates around WarBOT will be sent to this channel.")]
        CH_WarBot_Updates = 1,
        [Description("WAR Notifications will be sent here. War prep started, war prep ending... etc.")]
        CH_WAR_Announcements = 2,
        [Description("Functions requring officer/admins will be directed here. Clan management functions... etc.")]
        CH_Officers = 3,
        [Description("If I am configured to welcome new users, I will do it here.")]
        CH_User_Join = 4,
        [Description("If a user leaves the guild, I will send the message to here.")]
        CH_User_Left = 5,
    }
}

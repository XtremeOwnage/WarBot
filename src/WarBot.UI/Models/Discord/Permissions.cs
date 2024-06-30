namespace WarBot.UI.Models.Discord
{
    [Flags]
    public enum Permissions : long
    {
        CREATE_INVITE = 1,
        KICK_MEMBERS = 2,
        BAN_MEMBERS = 4,
        ADMINISTRATOR = 8,
        MANAGE_CHANNELS = 10,
        MANAGE_GUILD = 20,

    }
}

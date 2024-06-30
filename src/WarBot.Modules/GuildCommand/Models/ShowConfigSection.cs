namespace WarBot.Modules.GuildCommand.Models;
[Flags]
public enum ShowConfigSection
{
    Basic = 1,
    Roles = 2,
    Channels = 4,
    Events = 8,
    Hustle_War = 16,
    Hustle_Portal = 32,
    Hustle_Expedition = 64,

    ALL = Basic | Roles | Channels | Events | Hustle_All,
    Hustle_All = Hustle_War | Hustle_Portal | Hustle_Expedition,
}


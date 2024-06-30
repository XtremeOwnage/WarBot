using WarBot.DataAccess.Logic.Base;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.DataAccess.Logic;
public class HustleSettingLogic : LogicBase<HustleCastleSettings>
{
    internal HustleSettingLogic(GuildLogic parentLogic, HustleCastleSettings Entity) : base(parentLogic, Entity) { }

    public string? LootMessage
    {
        get => entity.LootMessage;
        set => entity.LootMessage = value;
    }

    public GuildChannelEventLogic Event_Portal => LoadRelatedLogic(o => o.Portal, o => new GuildChannelEventLogic(GuildLogic, o));

    public HustleGuildChannelEventLogic Event_War_1 => LoadRelatedLogic(o => o.War_1, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_War_2 => LoadRelatedLogic(o => o.War_2, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_War_3 => LoadRelatedLogic(o => o.War_3, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_War_4 => LoadRelatedLogic(o => o.War_4, o => new HustleGuildChannelEventLogic(GuildLogic, o));


    public HustleGuildChannelEventLogic Event_Expedition_1 => LoadRelatedLogic(o => o.Expedition_1, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_Expedition_2 => LoadRelatedLogic(o => o.Expedition_2, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_Expedition_3 => LoadRelatedLogic(o => o.Expedition_3, o => new HustleGuildChannelEventLogic(GuildLogic, o));
    public HustleGuildChannelEventLogic Event_Expedition_4 => LoadRelatedLogic(o => o.Expedition_4, o => new HustleGuildChannelEventLogic(GuildLogic, o));

    /// <summary>
    /// Reset and clear all configured settings.
    /// </summary>
    public override void ClearSettings()
    {
        Event_War_1.ClearSettings();
        Event_War_2.ClearSettings();
        Event_War_3.ClearSettings();
        Event_War_4.ClearSettings();
        Event_Expedition_1.ClearSettings();
        Event_Expedition_2.ClearSettings();
        Event_Expedition_3.ClearSettings();
        Event_Expedition_4.ClearSettings();
        Event_Portal.ClearSettings();
        LootMessage = null;
    }
    public HustleGuildChannelEventLogic GetWar(byte WarNo) => WarNo switch
    {
        1 => Event_War_1,
        2 => Event_War_2,
        3 => Event_War_3,
        4 => Event_War_4,
        _ => throw new ArgumentOutOfRangeException("There are only 4 wars. The value passed was not between 1 and 4.")
    };

    public HustleGuildChannelEventLogic GetExpedition(byte WarNo) => WarNo switch
    {
        1 => Event_Expedition_1,
        2 => Event_Expedition_2,
        3 => Event_Expedition_3,
        4 => Event_Expedition_4,
        _ => throw new ArgumentOutOfRangeException("There are only 4 expeditions. The value passed was not between 1 and 4.")
    };

}

using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic.Events;
public abstract class BaseGuildChannelEventLogic<T> : LogicBase<T> where T : GuildChannelEvent
{
    protected BaseGuildChannelEventLogic(GuildLogic parentLogic, T ChannelEvent) : base(parentLogic, ChannelEvent) { }

    public ChannelLogic Channel
    {
        get
        {
            db.Entry(entity).Reference(o => o.Channel).Load();
            return new ChannelLogic(GuildLogic, entity.Channel);
        }
    }


    public bool Enabled
    {
        get => entity.Enabled;
        set => entity.Enabled = value;
    }

    public string? Message
    {
        get => entity?.Message;
        set => entity.Message = value;
    }

    public override void ClearSettings()
    {
        this.Enabled = false;
        this.Message = null;
        this.Event_Enabled = false;
        this.Event_Title = null;
        this.Event_Description = null;
        this.ClearMethod = EventClearType.DISABLED;
        this.ClearDurationMins = 60;
        this.Channel.ClearSettings();
    }

    public bool Event_Enabled
    {
        get => entity.CreateEvent;
        set => entity.CreateEvent = value;
    }

    public string? Event_Title
    {
        get => entity.EventTitle;
        set => entity.EventTitle = value;
    }

    public string? Event_Description
    {
        get => entity.EventDescription;
        set => entity.EventDescription = value;
    }

    public EventClearType ClearMethod
    {
        get => !entity.ClearMethod.HasValue ? EventClearType.DISABLED : (EventClearType)entity.ClearMethod.Value;
        set => entity.ClearMethod = (uint)value;
    }

    public int ClearDurationMins
    {
        get => entity.ClearAfterMins ?? 60;
        set => entity.ClearAfterMins = value;
    }
}

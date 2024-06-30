using System.ComponentModel;

namespace WarBot.DataAccess.Logic.Events;
public enum EventClearType
{
    [Description("Don't clear messages")]
    DISABLED = 0,

    [Description("Clear each message individually after the specified duration has passed.")]
    INDIVIDUAL_MESSAGE_TIMER = 1,

    [Description("Clear the entire channel after the specified duration after the event has started.")]
    ENTIRE_CHANNEL = 2,
}


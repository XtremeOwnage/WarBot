using System;

namespace WarBot.Core;
public static class WarTimeHelper
{
    public class WarTimes
    {
        public WarTimes(byte EventNo, int UTC_START_HOUR)
        {
            this.WarNo = EventNo;
            var startTime = new TimeOnly(UTC_START_HOUR, 0);

            this.UTC_EventStart = startTime;
            this.UTC_PrepEnding_Hour = startTime.AddMinutes(-WarTimeHelper.PrepEndingMinsBeforeWar);
            this.UTC_PrepStart_Hour = startTime.AddHours(-WarTimeHelper.PrepHoursBeforeStart);
            this.UTC_DiscordEvent_Hour = startTime.AddHours(-WarTimeHelper.EventHoursBeforeStart);
        }

        public byte WarNo { get; init; }
        public TimeOnly UTC_EventStart { get; private set; }
        public TimeOnly UTC_PrepStart_Hour { get; private set; }
        public TimeOnly UTC_PrepEnding_Hour { get; private set; }
        public TimeOnly UTC_DiscordEvent_Hour { get; private set; }
    }

    public static WarTimes War_1 => new WarTimes(1, 9);
    public static WarTimes War_2 => new WarTimes(2, 15);
    public static WarTimes War_3 => new WarTimes(3, 21);
    public static WarTimes War_4 => new WarTimes(4, 3);

    public static WarTimes Expedition_1 => new WarTimes(1, 7);
    public static WarTimes Expedition_2 => new WarTimes(2, 13);
    public static WarTimes Expedition_3 => new WarTimes(3, 19);
    public static WarTimes Expedition_4 => new WarTimes(4, 1);

    public const int EventHoursBeforeStart = 4;
    public const int PrepHoursBeforeStart = 2;
    public const int PrepEndingMinsBeforeWar = 15;

    public static WarTimes GetWar(byte WarNo) => WarNo switch
    {
        1 => War_1,
        2 => War_2,
        3 => War_3,
        4 => War_4,
        _ => throw new System.Exception($"{WarNo} is not a valid war number")
    };

    public static WarTimes GetExpedition(byte EventNo) => EventNo switch
    {
        1 => Expedition_1,
        2 => Expedition_2,
        3 => Expedition_3,
        4 => Expedition_4,
        _ => throw new System.Exception($"{EventNo} is not a valid expedition number")
    };

    /// <summary>
    /// Returns a DateTimeOffset of the NEXT time provided in <paramref name="UTCHour"/>.
    /// </summary>
    /// <param name="UTCHour"></param>
    /// <returns></returns>
    public static DateTimeOffset GetNextOccuranceDT(int UTCHour)
    {
        var now = DateTimeOffset.UtcNow;
        var dt = new DateTimeOffset(now.Year, now.Month, now.Day, UTCHour, 0, 0, now.Offset);
        if (now > dt)
            return dt.AddDays(1);
        return dt;
    }

    /// <summary>
    /// Returns a DateTimeOffset of the NEXT time provided in <paramref name="Time"/>.
    /// </summary>
    /// <returns></returns>
    public static DateTimeOffset GetNextOccuranceDT(TimeOnly Time)
    {
        var now = DateTimeOffset.UtcNow;
        var dt = new DateTimeOffset(now.Year, now.Month, now.Day, Time.Hour, Time.Minute, Time.Second, now.Offset);
        if (now > dt)
            return dt.AddDays(1);
        return dt;
    }
}


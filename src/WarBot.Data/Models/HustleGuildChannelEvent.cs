#nullable enable
namespace WarBot.Data.Models;
/// <summary>
/// This table holds settings specific to hustle castle.
/// </summary>
public class HustleGuildChannelEvent : GuildChannelEvent
{
    public string? Prep_Started_Message { get; set; } = null;
    public string? Prep_Ending_Message { get; set; } = null;
    public string? Event_Started_Message { get; set; } = null;
    public string? Event_Finished_Message { get; set; } = null;

    /// <summary>
    /// How many minutes before EventStarted, until Prep Ending event is triggered.
    /// </summary>
    public int Prep_Ending_Mins { get; set; } = 15;

}


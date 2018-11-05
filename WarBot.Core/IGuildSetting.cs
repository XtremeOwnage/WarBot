namespace WarBot.Core
{
    public interface IGuildSetting
    {
        bool Enable { get; set; }
        string Value { get; set; }
    }
}
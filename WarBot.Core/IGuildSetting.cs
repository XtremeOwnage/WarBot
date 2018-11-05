namespace WarBot.Core
{
    public interface IGuildSetting
    {
        Setting_Key Key { get; }
        bool Enabled { get; set; }
        string Value { get; set; }
        /// <summary>
        /// Returns if Value is not null or whitespace.
        /// </summary>
        bool HasValue { get; }
        /// <summary>
        /// Sets both Enabled, and the Value
        /// </summary>
        void Set(bool Enable, string Value);
        /// <summary>
        /// Sets Enabled to True;
        /// </summary>
        void Enable();
        /// <summary>
        /// Sets Enabled to False;
        /// </summary>
        void Disable();
        /// <summary>
        /// Sets the value to null. Does not change Enabled.
        /// </summary>
        void Clear();
    }
}
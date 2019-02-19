namespace WarBot.Core.Updates
{
    /// <summary>
    /// This interface defines an update to the warbot logic.
    /// </summary>
    public interface IBotUpdate
    {
        /// <summary>
        /// The new version.
        /// </summary>
        double Version { get; }
        /// <summary>
        /// This holds the URL to see the change notes.
        /// </summary>
        string ReleaseNotesURL { get; }

        /// <summary>
        /// Determines if this update is worthy to be sent to the guild's update channel.
        /// </summary>
        bool SendUpdateToGuild { get; }

        /// <summary>
        /// Updates the configuration. 
        /// Beware - this will save the changes.
        /// </summary>
        /// <param name="cfg"></param>
        void Apply(IGuildConfig cfg);
    }
}

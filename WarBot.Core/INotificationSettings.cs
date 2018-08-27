namespace WarBot.Core
{
    public interface INotificationSettings
    {
        bool SendUpdateMessage { get; set; }
        bool War1Enabled { get; set; }
        bool War2Enabled { get; set; }
        bool War3Enabled { get; set; }
        bool War4Enabled { get; set; }
        bool WarPrepEnding { get; set; }
        string WarPrepEndingMessage { get; set; }
        bool WarPrepStarted { get; set; }
        string WarPrepStartedMessage { get; set; }
        bool WarStarted { get; set; }
        string WarStartedMessage { get; set; }
    }
}
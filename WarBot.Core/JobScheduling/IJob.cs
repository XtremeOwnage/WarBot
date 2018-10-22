namespace WarBot.Core.JobScheduling
{
    /// <summary>
    /// Represents a job, from the job scheduling sub-system.
    /// </summary>
    public interface IJob
    {
        string ID { get; }
        IJobScheduler Scheduler { get; }
        bool Delete();
    }
}

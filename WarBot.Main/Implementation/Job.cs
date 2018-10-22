using Quartz;
using WarBot.Core.JobScheduling;

namespace WarBot.Implementation
{
    public class Job : Core.JobScheduling.IJob
    {
        public Job(string ID, IJobScheduler JOB)
        {
            this.ID = ID;
            this.Scheduler = JOB;
        }
        public Job(IJobDetail Job, IJobScheduler JOB)
        {
            this.ID = Job.Key.Name;
            this.Scheduler = JOB;
        }
        public string ID { get; private set; }

        public IJobScheduler Scheduler { get; private set; }

        public override string ToString() => ID;

        public bool Delete() => Scheduler.Delete(this.ID).Result;
    }
}

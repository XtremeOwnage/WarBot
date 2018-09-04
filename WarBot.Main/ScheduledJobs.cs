using Hangfire;
using Hangfire.Storage;
using WarBot.Core.JobScheduling;
using WarBot.Util;

namespace WarBot
{
    public static class ScheduledJobs
    {
        /// <summary>
        /// Schedule all of the recurring jobs.
        /// </summary>
        /// <param name="job"></param>
        public static void ScheduleJobs(IJobScheduler job)
        {
            //Delete all current jobs.
            using (var connection = JobStorage.Current.GetConnection())
            {
                foreach (var recurringJob in connection.GetRecurringJobs())
                {
                    RecurringJob.RemoveIfExists(recurringJob.Id);
                }
            }


            job.RecurringJob<WAR_Messages>("war1_prep_started", o => o.SendWarPrepStarted(1), "0 2 * * *");
            job.RecurringJob<WAR_Messages>("war2_prep_started", o => o.SendWarPrepStarted(2), "0 8 * * *");
            job.RecurringJob<WAR_Messages>("war3_prep_started", o => o.SendWarPrepStarted(3), "0 14 * * *");
            job.RecurringJob<WAR_Messages>("war4_prep_started", o => o.SendWarPrepStarted(4), "0 20 * * *");

            job.RecurringJob<WAR_Messages>("war1_prep_ending", o => o.SendWarPrepEnding(1), "45 3 * * *");
            job.RecurringJob<WAR_Messages>("war2_prep_ending", o => o.SendWarPrepEnding(2), "45 9 * * *");
            job.RecurringJob<WAR_Messages>("war3_prep_ending", o => o.SendWarPrepEnding(3), "45 15 * * *");
            job.RecurringJob<WAR_Messages>("war4_prep_ending", o => o.SendWarPrepEnding(4), "45 21 * * *");

            job.RecurringJob<WAR_Messages>("war1_started", o => o.SendWarStarted(1), "0 4 * * *");
            job.RecurringJob<WAR_Messages>("war2_started", o => o.SendWarStarted(2), "0 10 * * *");
            job.RecurringJob<WAR_Messages>("war3_started", o => o.SendWarStarted(3), "0 16 * * *");
            job.RecurringJob<WAR_Messages>("war4_started", o => o.SendWarStarted(4), "0 22 * * *");
        }
    }
}

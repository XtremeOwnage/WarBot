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
            //War prep started
            job.RecurringJob<WAR_Messages>("war1_prep_started", o => o.SendWarPrepStarted(1).Wait(), "0 0 7 * * ?");
            job.RecurringJob<WAR_Messages>("war2_prep_started", o => o.SendWarPrepStarted(2).Wait(), "0 0 13 * * ?");
            job.RecurringJob<WAR_Messages>("war3_prep_started", o => o.SendWarPrepStarted(3).Wait(), "0 0 19 * * ?");
            job.RecurringJob<WAR_Messages>("war4_prep_started", o => o.SendWarPrepStarted(4).Wait(), "0 0 1 * * ?");

            //War Prep Ending
            job.RecurringJob<WAR_Messages>("war1_prep_ending", o => o.SendWarPrepEnding(1).Wait(), "0 45 8 * * ?");
            job.RecurringJob<WAR_Messages>("war2_prep_ending", o => o.SendWarPrepEnding(2).Wait(), "0 45 14 * * ?");
            job.RecurringJob<WAR_Messages>("war3_prep_ending", o => o.SendWarPrepEnding(3).Wait(), "0 45 20 * * ?");
            job.RecurringJob<WAR_Messages>("war4_prep_ending", o => o.SendWarPrepEnding(4).Wait(), "0 45 2 * * ?");

            //War Started
            job.RecurringJob<WAR_Messages>("war1_started", o => o.SendWarStarted(1).Wait(), "0 0 9 * * ?");
            job.RecurringJob<WAR_Messages>("war2_started", o => o.SendWarStarted(2).Wait(), "0 0 15 * * ?");
            job.RecurringJob<WAR_Messages>("war3_started", o => o.SendWarStarted(3).Wait(), "0 0 21 * * ?");
            job.RecurringJob<WAR_Messages>("war4_started", o => o.SendWarStarted(4).Wait(), "0 0 3 * * ?");

            //Process reactions queue
            job.RecurringJob_EveryNSeconds<WARBOT>("process_reactions", o => o.ProcessReactions(), 15);

            //Send Portal Started
            job.RecurringJob<WAR_Messages>("portal_opened", o => o.SendPortalOpened().Wait(), "0 0 9 ? * FRI");

            //Update the Discord bot list every two hours.
            job.RecurringJob<WARBOT>("update_dbl", o => o.TaskBot.Update_DiscordBotList().Wait(), "0 0 */2 * * ?");

            //Update the Discord bot list every two hours.
            job.RecurringJob<WARBOT>("update_status", o => o.TaskBot.Update_Status(null).Wait(), "0 0/5 0 ? * * *");
        }
    }
}

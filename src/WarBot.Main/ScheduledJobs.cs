using Hangfire;
using System.Linq.Expressions;
using WarBot.Core;
using WarBot.Modules.Command.Models;
using WarBot.Modules.Jobs;

namespace WarBot
{
    public static class ScheduledJobs
    {
        /// <summary>
        /// Schedule all of the recurring jobs.
        /// </summary>
        /// <param name="job"></param>
        public static void ScheduleJobs()
        {
            foreach (byte i in Enumerable.Range(1, 4))
            {
                //Hustle War Events
                var t = WarTimeHelper.GetWar(i);
                AddJobUTC<JobHustleWar>($"war{i}_discord_event", o => o.CreateDiscordEvents(i), t.UTC_DiscordEvent_Hour);
                AddJobUTC<JobHustleWar>($"war{i}_prep_started", o => o.SendWarPrepStarted(i), t.UTC_PrepStart_Hour);
                AddJobUTC<JobHustleWar>($"war{i}_prep_ending", o => o.SendWarPrepEnding(i), t.UTC_PrepEnding_Hour);
                AddJobUTC<JobHustleWar>($"war{i}_started", o => o.SendWarStarted(i), t.UTC_EventStart);

                //Hustle Expeditions
                t = WarTimeHelper.GetExpedition(i);
                AddJobUTC<JobHustleExpedition>($"expedition{i}_discord_event", o => o.CreateDiscordEvents(i), t.UTC_DiscordEvent_Hour);
                AddJobUTC<JobHustleExpedition>($"expedition{i}_prep_started", o => o.SendWarPrepStarted(i), t.UTC_PrepStart_Hour);
                AddJobUTC<JobHustleExpedition>($"expedition{i}_prep_ending", o => o.SendWarPrepEnding(i), t.UTC_PrepEnding_Hour);
                AddJobUTC<JobHustleExpedition>($"expedition{i}_started", o => o.SendWarStarted(i), t.UTC_EventStart);
            }


            //Process reactions queue
            // RecurringJob.AddOrUpdate<WARBOT>("process_reactions", o => o.ProcessReactions(), 15);

            //Send Portal Started
            AddJobUTC<JobHustleWar>(JobList.portal_opened, o => o.SendPortalOpened(), "0 0 9 ? * FRI");

            //Update WarBOT's status every minute.
            AddJobUTC<UpdateStatus>(JobList.update_status, o => o.Execute(null), "0 0/1 * * * ?");

            //Register discord commands.
            AddJobUTC<RegisterDiscordCommandsJob>(JobList.discord_register_commands, o => o.ExecuteAsync(), "0 0 1 1 1 ?");

            //Check every 6 hours to ensure we have the applications.commands scope, upon startup.
            AddJobUTC<CheckDiscordScopes>(JobList.discord_check_scopes, o => o.Execute(), "0 0 */6 * * ?");

            //Register commands on startup.
            //Don't do this.... Commands will stay until they are overwritten.
            //BackgroundJob.Schedule<RegisterDiscordCommandsJob>(o => o.ExecuteAsync(), TimeSpan.FromSeconds(5));

            //Check scopes upon startup.
            //BackgroundJob.Schedule<CheckDiscordScopes>(o => o.Execute(), TimeSpan.FromSeconds(5));

        }

        private static JobList ParseJobName(string jobName)
        {
            if (Enum.TryParse<JobList>(jobName, out var job))
                return job;
            else throw new ArgumentException($"Unable to parse JobList from '{jobName}'");
        }
        private static void AddJobUTC<T>(JobList job, Expression<Func<T, Task>> methodCall, string cronExpression)
        {
            RecurringJob.AddOrUpdate(job.ToString(), methodCall, cronExpression, new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
        }
        private static string CronFromTime(TimeOnly time) => $"0 {time.Minute} {time.Hour} * * ?";

        private static void AddJobUTC<T>(string jobName, Expression<Func<T, Task>> methodCall, TimeOnly Time)
            => AddJobUTC(ParseJobName(jobName), methodCall, CronFromTime(Time));
    }
}

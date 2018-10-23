using Ninject;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WarBot.Core.JobScheduling;

namespace WarBot.Implementation
{
    public class QuartzJobScheduler : IJobScheduler
    {
        IScheduler scheduler;
        IKernel kernel;
        public QuartzJobScheduler(WARBOT bot)
        {
            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            scheduler = factory.GetScheduler().Result;

            scheduler.Start(bot.StopToken.Token);
        }

        Task<bool> IJobScheduler.Delete(string jobId) => scheduler.DeleteJob(JobKey.Create(jobId));
        Task<bool> IJobScheduler.Delete(Core.JobScheduling.IJob job) => scheduler.DeleteJob(JobKey.Create(job.ID));


        public async Task<Core.JobScheduling.IJob> Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
        {
            try
            {
                var Action = methodCall.Compile();

                var job = Job_Action<T>.Create(Action);
                ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                    .StartAt(DateTime.UtcNow.Add(delay))
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                return new Job(job, this);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Core.JobScheduling.IJob> RecurringJob<T>(string jobID, Expression<Action<T>> Expression, string cronSchedule)
        {
            try
            {
                var Action = Expression.Compile();

                var job = Job_Action<T>.Create(Action);
                var trigger = TriggerBuilder.Create()
                    .WithCronSchedule(cronSchedule)
                    .ForJob(job)
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                return new Job(job, this);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}

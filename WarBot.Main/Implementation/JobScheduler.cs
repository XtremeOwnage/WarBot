using Ninject;
using Quartz;
using Quartz.Impl;
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
            var Action = methodCall.Compile();

            var job = Job_Action<T>.Create(Action);
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .ForJob(job)
                .StartAt(DateTime.UtcNow.Add(delay))
                .Build();

            await scheduler.ScheduleJob(trigger);

            return new Job(job, this);
        }

        public async Task<Core.JobScheduling.IJob> RecurringJob<T>(string jobID, Expression<Action<T>> Expression, string cronSchedule)
        {
            var Action = Expression.Compile();

            var job = Job_Action<T>.Create(Action);
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .ForJob(job)
                .WithCronSchedule(cronSchedule)
                .Build();

            await scheduler.ScheduleJob(trigger);

            return new Job(job, this);
        }

        public async Task<Core.JobScheduling.IJob> RecurringJob(string jobID, Expression<Action> Expression, string cronSchedule)
        {
            var Action = Expression.Compile();

            var job = Job_Action.Create(Action);

            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .ForJob(job)
                .WithCronSchedule(cronSchedule)
                .Build();

            await scheduler.ScheduleJob(trigger);

            return new Job(job, this);
        }
    }
}

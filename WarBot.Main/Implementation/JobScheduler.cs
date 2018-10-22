using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using WarBot.Core;
using WarBot.Core.JobScheduling;

namespace WarBot.Implementation
{
    public class HangfireJobScheduler : IJobScheduler
    {
        bool IJobScheduler.Delete(string jobId) => BackgroundJob.Delete(jobId);
        bool IJobScheduler.Delete(string jobId, string fromState) => BackgroundJob.Delete(jobId, fromState);
        bool IJobScheduler.Requeue(string jobId) => BackgroundJob.Requeue(jobId);
        bool IJobScheduler.Requeue(string jobId, string fromState) => BackgroundJob.Requeue(jobId, fromState);
        IJob IJobScheduler.ContinueWith(string parentId, Expression<Action> methodCall)
            => new Job(BackgroundJob.ContinueWith(parentId, methodCall, JobContinuationOptions.OnAnyFinishedState), this);
        IJob IJobScheduler.ContinueWith<T>(string parentId, Expression<Action<T>> methodCall)
            => new Job(BackgroundJob.ContinueWith<T>(parentId, methodCall), this);
        IJob IJobScheduler.Enqueue(Expression<Action> methodCall)
            => new Job(BackgroundJob.Enqueue(methodCall), this);
        IJob IJobScheduler.Enqueue<T>(Expression<Action<T>> methodCall)
            => new Job(BackgroundJob.Enqueue(methodCall), this);
        IJob IJobScheduler.Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt)
            => new Job(BackgroundJob.Schedule(methodCall, enqueueAt), this);
        IJob IJobScheduler.Schedule(Expression<Action> methodCall, TimeSpan delay)
            => new Job(BackgroundJob.Schedule(methodCall, delay), this);
        IJob IJobScheduler.Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt)
            => new Job(BackgroundJob.Schedule(methodCall, enqueueAt), this);
        IJob IJobScheduler.Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay)
            => new Job(BackgroundJob.Schedule(methodCall, delay), this);

        void IJobScheduler.RecurringJob<T>(string jobID, Expression<Action<T>> Expression, string cronSchedule)
            => Hangfire.RecurringJob.AddOrUpdate(jobID, Expression, cronSchedule, TimeZoneInfo.Local);

        void IJobScheduler.RecurringJob(string jobID, Expression<Action> Expression, string cronSchedule)
            => Hangfire.RecurringJob.AddOrUpdate(jobID, Expression, cronSchedule, TimeZoneInfo.Local);
    }
}

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace WarBot.Core.JobScheduling
{
    /// <summary>
    /// Represents a job, from the job scheduling sub-system.
    /// </summary>
    public interface IJob
    {
        string ID { get; }
        IJobScheduler Scheduler { get; }

        IJob ContinueWith(Expression<Action> methodCall);
        IJob ContinueWith<T>(Expression<Action<T>> methodCall);

        bool Delete(string jobId);
        bool Delete(string jobId, string fromState);
        bool Requeue(string jobId);
        bool Requeue(string jobId, string fromState);
    }
}

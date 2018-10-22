using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WarBot.Core.JobScheduling
{    /// <summary>
     /// This class abstracts the job scheduling method calls, from the implementation. Will allow us
     /// to easily switch providers in the future.
     /// </summary>
    public interface IJobScheduler
    {
        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.DeletedState. Hangfire.BackgroundJobClientExtensions.Delete(Hangfire.IBackgroundJobClient,System.String)
        //
        // Parameters: jobId: An identifier, that will be used to find a job.
        //
        // Returns: True on a successfull state transition, false otherwise.
        Task<bool> Delete(string jobId);

        Task<bool> Delete(IJob job);

        // Summary: Creates a new background job based on a specified instance method call expression
        // and schedules it to be enqueued after a given delay.
        //
        // Parameters: methodCall: Instance method call expression that will be marshalled to the Server.
        //
        // delay: Delay, after which the job will be enqueued.
        //
        // Type parameters: T: Type whose method will be invoked during job processing.
        //
        // Returns: Unique identifier of the created job.
        Task<IJob> Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);

        //Schedule recurring job.
        Task<IJob> RecurringJob<T>(string jobID, Expression<Action<T>> Expression, string cronSchedule);

        Task<IJob> RecurringJob(string jobID, Expression<Action> Expression, string cronSchedule);
    }
}

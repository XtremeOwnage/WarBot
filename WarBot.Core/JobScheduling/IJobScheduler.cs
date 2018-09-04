using System;
using System.Linq.Expressions;

namespace WarBot.Core.JobScheduling
{    /// <summary>
     /// This class abstracts the job scheduling method calls, from the implementation. Will allow us
     /// to easily switch providers in the future.
     /// </summary>
    public interface IJobScheduler
    {
        // Summary: Creates a new background job that will wait for another background job to be enqueued.
        //
        // Parameters: parentId: Identifier of a background job to wait completion for.
        //
        // methodCall: Method call expression that will be marshalled to a server.
        //
        // options: Continuation options.
        //
        // Returns: Unique identifier of a created job.
        IJob ContinueWith<T>(string parentId, Expression<Action<T>> methodCall);


        // Summary: Creates a new background job that will wait for another background job to be enqueued.
        //
        // Parameters: parentId: Identifier of a background job to wait completion for.
        //
        // methodCall: Method call expression that will be marshalled to a server.
        //
        // options: Continuation options.
        //
        // Returns: Unique identifier of a created job.
        IJob ContinueWith(string parentId, Expression<Action> methodCall);


        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.DeletedState. State change is only performed if current job state is equal
        // to the fromState value. Hangfire.BackgroundJobClientExtensions.Delete(Hangfire.IBackgroundJobClient,System.String,System.String)
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // fromState: Current state assertion, or null if unneeded.
        //
        // Returns: True, if state change succeeded, otherwise false.
        bool Delete(string jobId, string fromState);

        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.DeletedState. Hangfire.BackgroundJobClientExtensions.Delete(Hangfire.IBackgroundJobClient,System.String)
        //
        // Parameters: jobId: An identifier, that will be used to find a job.
        //
        // Returns: True on a successfull state transition, false otherwise.
        bool Delete(string jobId);

        IJob Enqueue<T>(Expression<Action<T>> methodCall);


        // Summary: Creates a new fire-and-forget job based on a given method call expression.
        //
        // Parameters: methodCall: Method call expression that will be marshalled to a server.
        //
        // Returns: Unique identifier of a background job.
        //
        // Exceptions: T:System.ArgumentNullException: methodCall is null.
        IJob Enqueue(Expression<Action> methodCall);

        // Summary: Changes state of a job with the specified jobId to the Hangfire.States.EnqueuedState.
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // Returns: True, if state change succeeded, otherwise false.
        bool Requeue(string jobId);

        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.EnqueuedState. If fromState value is not null, state change will be
        // performed only if the current state name of a job equal to the given value.
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // fromState: Current state assertion, or null if unneeded.
        //
        // Returns: True, if state change succeeded, otherwise false.
        bool Requeue(string jobId, string fromState);

        // Summary: Creates a new background job based on a specified method call expression and
        // schedules it to be enqueued at the given moment of time.
        //
        // Parameters: methodCall: Method call expression that will be marshalled to the Server.
        //
        // enqueueAt: The moment of time at which the job will be enqueued.
        //
        // Type parameters: T: The type whose method will be invoked during the job processing.
        //
        // Returns: Unique identifier of a created job.
        IJob Schedule<T>(Expression<Action<T>> methodCall, DateTimeOffset enqueueAt);

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
        IJob Schedule<T>(Expression<Action<T>> methodCall, TimeSpan delay);


        // Summary: Creates a new background job based on a specified method call expression and
        // schedules it to be enqueued at the given moment of time.
        //
        // Parameters: methodCall: Method call expression that will be marshalled to the Server.
        //
        // enqueueAt: The moment of time at which the job will be enqueued.
        //
        // Returns: Unique identifier of a created job.
        IJob Schedule(Expression<Action> methodCall, DateTimeOffset enqueueAt);

        // Summary: Creates a new background job based on a specified method call expression and
        // schedules it to be enqueued after a given delay.
        //
        // Parameters: methodCall: Instance method call expression that will be marshalled to the Server.
        //
        // delay: Delay, after which the job will be enqueued.
        //
        // Returns: Unique identifier of the created job.
        IJob Schedule(Expression<Action> methodCall, TimeSpan delay);

        //Schedule recurring job.
        void RecurringJob<T>(string jobID, Expression<Action<T>> Expression, string cronSchedule);

        void RecurringJob(string jobID, Expression<Action> Expression, string cronSchedule);
    }
}

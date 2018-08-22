using System;
using System.Linq.Expressions;
using WarBot.Core.JobScheduling;

namespace WarBot.Implementation
{
    public class Job : IJob
    {
        public Job(string ID, IJobScheduler JOB)
        {
            this.ID = ID;
            this.Scheduler = JOB;
        }
        public string ID { get; private set; }

        public IJobScheduler Scheduler { get; private set; }

        public override string ToString()
        {
            return this.ID;
        }

        // Summary: Creates a new background job that will wait for another background job to be enqueued.
        //
        // Parameters: parentId: Identifier of a background job to wait completion for.
        //
        // methodCall: Method call expression that will be marshalled to a server.
        //
        // options: Continuation options.
        //
        // Returns: Unique identifier of a created job.
        public IJob ContinueWith<T>(Expression<Action<T>> methodCall)
            => Scheduler.ContinueWith(this.ID, methodCall);
        // Summary: Creates a new background job that will wait for another background job to be enqueued.
        //
        // Parameters: parentId: Identifier of a background job to wait completion for.
        //
        // methodCall: Method call expression that will be marshalled to a server.
        //
        // options: Continuation options.
        //
        // Returns: Unique identifier of a created job.
        public IJob ContinueWith(Expression<Action> methodCall)
            => Scheduler.ContinueWith(this.ID, methodCall);


        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.DeletedState. State change is only performed if current job state is equal
        // to the fromState value. Hangfire.BackgroundJobClientExtensions.Delete(Hangfire.IBackgroundJobClient,System.String,System.String)
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // fromState: Current state assertion, or null if unneeded.
        //
        // Returns: True, if state change succeeded, otherwise false.
        public bool Delete(string jobId, string fromState)
             => Scheduler.Delete(this.ID, fromState);

        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.DeletedState. Hangfire.BackgroundJobClientExtensions.Delete(Hangfire.IBackgroundJobClient,System.String)
        //
        // Parameters: jobId: An identifier, that will be used to find a job.
        //
        // Returns: True on a successfull state transition, false otherwise.
        public bool Delete(string jobId)
            => Scheduler.Delete(this.ID);


        // Summary: Changes state of a job with the specified jobId to the Hangfire.States.EnqueuedState.
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // Returns: True, if state change succeeded, otherwise false.
        public bool Requeue(string jobId)
            => Scheduler.Requeue(this.ID);

        // Summary: Changes state of a job with the specified jobId to the
        // Hangfire.States.EnqueuedState. If fromState value is not null, state change will be
        // performed only if the current state name of a job equal to the given value.
        //
        // Parameters: jobId: Identifier of job, whose state is being changed.
        //
        // fromState: Current state assertion, or null if unneeded.
        //
        // Returns: True, if state change succeeded, otherwise false.
        public bool Requeue(string jobId, string fromState)
            => Scheduler.Requeue(this.ID, fromState);
    }
}

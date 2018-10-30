using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace WarBot.Core
{
    public static class ParallelActionHelper
    {
        /// <summary>
        /// Executes each action in an array, in parallel.
        /// Returns all exceptions which may have occured.
        /// </summary>
        /// <param name="Actions"></param>
        /// <returns></returns>
        public static Exception[] executeParallel(this IEnumerable<Action> Actions, CancellationToken Token, int MaxJobs = 10)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            ParallelOptions options = new ParallelOptions
            {
                CancellationToken = Token,
                MaxDegreeOfParallelism = MaxJobs
            };
            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(Actions, options, d =>
            {
                try
                {
                    d.Invoke();
                }
                // Store the exception and continue with the loop.                    
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            return exceptions.ToArray();
        }
    }
}

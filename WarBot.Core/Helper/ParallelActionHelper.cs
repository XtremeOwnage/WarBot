using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public static Exception[] executeParallel(this IEnumerable<Action> Actions)
        {
            var exceptions = new ConcurrentQueue<Exception>();

            // Execute the complete loop and capture all exceptions.
            Parallel.ForEach(Actions, d =>
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

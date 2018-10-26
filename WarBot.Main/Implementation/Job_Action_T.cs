using Quartz;
using System;
using System.Threading.Tasks;
using Ninject;

namespace WarBot.Implementation
{
    public class Job_Action<T> : IJob
    {
        public static IJobDetail Create(string Identity, Action<T> action, bool Durable = false)
        {
            try
            {
                return JobBuilder
                    .Create<Job_Action<T>>()
                    .WithIdentity(new JobKey(Identity))
                    .StoreDurably(Durable)
                    .SetJobData(new JobDataMap
                    {
                        {"action", action}
                    })
                    .Build();
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var kernel = WARBOT.kernel;
                var instance = kernel.Get<T>();

                var action = context.MergedJobDataMap["action"] as Action<T>;

                return Task.Run(() => action.Invoke(instance));
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}

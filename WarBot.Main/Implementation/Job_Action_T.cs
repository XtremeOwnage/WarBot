using Quartz;
using System;
using System.Threading.Tasks;
using Ninject;

namespace WarBot.Implementation
{
    public class Job_Action<T> : IJob
    {
        public static IJobDetail Create(Action<T> action, bool Durable = false)
        {
            return JobBuilder
                .Create<Job_Action<T>>()
                .StoreDurably(Durable)
                .SetJobData(new JobDataMap
                {
                    {"action", action}
                })
                .Build();
        }
        public Task Execute(IJobExecutionContext context)
        {
            var kernel = WARBOT.kernel;
            var instance = kernel.Get<T>();

            var action = context.MergedJobDataMap["action"] as Action<T>;

            return Task.Run(() => action.Invoke(instance));
        }
    }
}

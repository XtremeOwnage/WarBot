using Quartz;
using System;
using System.Threading.Tasks;

namespace WarBot.Implementation
{
    public class Job_Action : Quartz.IJob
    {
        public static IJobDetail Create(Action action)
        {
            return JobBuilder
                .Create<Job_Action>()
                .SetJobData(new JobDataMap
                {
                    {"action", action}
                })
                .Build();
        }
        public Task Execute(IJobExecutionContext context)
        {
            var action = context.MergedJobDataMap["action"] as Action;
            return Task.Run(action);
        }
    }
}

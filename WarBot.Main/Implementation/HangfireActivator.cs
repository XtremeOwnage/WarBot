using System;

namespace WarBot.Implementation
{
    /// <summary>
    /// Allows Hangfire to use the ServiceProvider for dependancy injection.
    /// </summary>
    public class HangfireActivator : Hangfire.JobActivator
    {
        private readonly IServiceProvider _serviceProvider;

        public HangfireActivator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override object ActivateJob(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}

namespace WarBot.Core
{
    public static class MetricHelper
    {
        public static int UserContextInteractions => user;
        public static int SlashExecutions => slash;
        public static int MessageContextExecutions => message;
        public static int CustomCommandExecutions => custom;
        public static int InteractionFailures => interactionFailures;

        private static int slash = 0;
        private static int message = 0;
        private static int user = 0;
        private static int custom = 0;
        private static int interactionFailures = 0;

        public static void AddUserContextExecution() => System.Threading.Interlocked.Increment(ref user);
        public static void AddSlashCommandExecution() => System.Threading.Interlocked.Increment(ref slash);
        public static void AddMessageContextExecution() => System.Threading.Interlocked.Increment(ref message);
        public static void AddCustomExecution() => System.Threading.Interlocked.Increment(ref custom);
        public static void AddInteractionFailure() => System.Threading.Interlocked.Increment(ref interactionFailures);
    }
}

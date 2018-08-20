namespace WarBot.Core
{
    public interface ICommandService
    {
        int CommandCount { get; }
        int ModuleCount { get; }
    }
}
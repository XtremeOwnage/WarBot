
namespace WarBot.Modules.MenuComponents
{
    public interface IComponentResult<TResult>
    {
        bool Skipped { get; }

        TResult Value { get; }

        /// <summary>
        /// Disables the selected component.
        /// </summary>
        /// <returns></returns>
        Task Disable();

        /// <summary>
        /// Deletes the select component.
        /// </summary>
        /// <returns></returns>
        Task Delete();
    }
}
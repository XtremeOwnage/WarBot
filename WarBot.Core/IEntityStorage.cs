using Discord;

namespace WarBot.Core
{
    public interface IEntityStorage<U> where U : IEntity<ulong>
    {
        ulong ID { get; set; }
        string Name { get; set; }
        U Value { get; set; }
    }
}
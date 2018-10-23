using Discord;

namespace WarBot.Core
{
    public interface IStoredDiscordEntity<T> where T : class, IEntity<ulong>
    {
        ulong? EntityId { get; }
        int ID { get; }
        string Name { get; set; }
        T Value { get; set; }

        void Set(T Value);
        void Set(ulong ID, string Name);
    }
}
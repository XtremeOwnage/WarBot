using Discord;
using Newtonsoft.Json;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Storage
{
    public class EntityStorage<U> : IEntityStorage<U> where U : IEntity<ulong>
    {
        public string Name { get; set; }
        public ulong ID { get; set; }
        /// <summary>
        /// Holds a reference to the value, if it has been created and stored.
        /// </summary>
        [JsonIgnore, JsonProperty(Required = Required.Default)]
        public U Value { get; set; } = default(U);
        public EntityStorage() { }
        public EntityStorage(string Name, ulong ID)
        {
            this.Name = Name;
            this.ID = ID;
        }
        public EntityStorage(string Name, U Class)
        {
            this.ID = Class.Id;
            this.Value = Class;
        }

        public static EntityStorage<U> Channel<T>(T Instance)
            where T : IChannel, U
        {
            return new EntityStorage<U>(Instance.Name, Instance);
        }
        public static EntityStorage<U> Role<T>(T Instance)
            where T : IRole, U
        {
            return new EntityStorage<U>(Instance.Name, Instance);
        }
    }
    public static class EntityStorageExtensions
    {
        public static EntityStorage<IRole> CreateStorage(this IRole Role)
                => EntityStorage<IRole>.Role(Role);

        public async static Task<EntityStorage<IRole>> CreateStorage(this Task<IRole> Role)
            => CreateStorage(await Role);
        public static EntityStorage<ITextChannel> CreateStorage(this ITextChannel Channel)
                => EntityStorage<ITextChannel>.Channel(Channel);
        public async static Task<EntityStorage<ITextChannel>> CreateStorage(this Task<ITextChannel> Channel)
                => CreateStorage(await Channel);

    }
}

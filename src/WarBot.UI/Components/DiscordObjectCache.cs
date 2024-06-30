using Discord;
using Discord.Rest;
using Microsoft.Extensions.Caching.Memory;

namespace WarBot.UI.Components
{
    public class DiscordObjectCache
    {
        public DiscordObjectCache(DiscordRestClient client)
        {
            this.client = client;
        }

        private readonly DiscordRestClient client;
        
        private readonly MemoryCache channelCache = new MemoryCache(new MemoryCacheOptions());

        /// <summary>
        /// Returns a list of text channels for a given guild, WHERE WarBOT has the ability to send messages.
        /// </summary>
        /// <param name="GuildID"></param>
        /// <returns></returns>
        public async Task<List<ITextChannel>> GetGuildChannels(ulong GuildID)
        {
            if (channelCache.TryGetValue<List<ITextChannel>>(GuildID, out var cch))
                return cch;

            var guild = await client.GetGuildAsync(GuildID);
            var chans = await guild.GetTextChannelsAsync();
            var textChans = chans.OfType<ITextChannel>();

            var Channels = new List<ITextChannel>();
            foreach (var ch in textChans)
            {
                if (await ch.TestBotPermissionAsync(ChannelPermission.SendMessages))
                    Channels.Add(ch);
            }

            channelCache.Set(GuildID, Channels, DateTimeOffset.Now.AddMinutes(30));

            return Channels;
        }

        /// <summary>
        /// Refreshes the list of available channels from a guild, and returns the refreshed list.
        /// </summary>
        /// <param name="GuildID"></param>
        /// <returns></returns>
        public Task<List<ITextChannel>> RefreshChannels(ulong GuildID)
        {
            channelCache.Remove(GuildID);
            return GetGuildChannels(GuildID);
        }

    }
}

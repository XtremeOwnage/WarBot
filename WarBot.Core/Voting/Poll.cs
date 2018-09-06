using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace WarBot.Core.Voting
{
    public class Poll
    {
        IGuildConfig config;
        public Poll(IGuildConfig cfg, ulong Channel, string Question)
        {
            config = cfg;
            this.ChannelId = Channel;
            this.Question = Question;
        }
        public string Question { get; }
        public Dictionary<IEmote, string> EmojiOption { get; } = new Dictionary<IEmote, string>();
        public ConcurrentDictionary<ulong, IEmote> UserVotes { get; } = new ConcurrentDictionary<ulong, IEmote>();
        public ulong ChannelId { get; }

        public IUserMessage Message { get; set; }
        public ulong MessageId => Message.Id;


        public async Task End()
        {
            var eb = new EmbedBuilder()
                .WithTitle($"Poll Results: {Question}");

            var Results = UserVotes
                 .GroupBy(o => o.Value)
                 .Select(o => new
                 {
                     Votes = o.Count(),
                     Question = EmojiOption[o.Key],
                     Emoji = o.Key,
                 })
                 .Where(o => o.Votes > 0)
                 .OrderByDescending(o => o.Votes);

            foreach (var r in Results)
                eb.AddField($"Votes: {r.Votes}", r.Question);


            await Message.Channel.SendMessageAsync(embed: eb.Build());
        }
    }
}

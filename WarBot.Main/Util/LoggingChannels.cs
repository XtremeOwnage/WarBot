using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using WarBot.Core;

namespace WarBot.Util
{
    public class LoggingChannels
    {
        public SocketTextChannel Chat { get; set; }
        public SocketTextChannel Error { get; set; }
        public SocketTextChannel Activity { get; set; }
        public SocketTextChannel Debug { get; set; }

        public void AddLoggingChannel(Environment env, LogChannel ch, ulong CHID)
        {
            channels.Add(new KeyAggregate
            {
                Env = env,
                Channel = ch,
                ChannelId = CHID
            });
        }
        public ulong GetChannelID(Environment env, LogChannel ch) => channels.First(o => o.Channel == ch && o.Env == env).ChannelId;
        public bool IsLoggingChannel(ulong CHID) => channels.Any(o => o.ChannelId == CHID);

        private class KeyAggregate
        {
            public Environment Env;
            public LogChannel Channel;
            public ulong ChannelId;
        }
        private HashSet<KeyAggregate> channels { get; set; } = new HashSet<KeyAggregate>();
    }
}

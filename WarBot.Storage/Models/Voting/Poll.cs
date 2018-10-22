using Discord;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarBot.Core;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace WarBot.Storage.Models.Voting
{
    public class Poll
    {
        /// <summary>
        /// Parameter-less constructor for entity framework.
        /// </summary>
        public Poll() { }
        public Poll(IMessageChannel Channel, string Question)
        {
            this.Channel = Channel;
            this.Question = Question;
        }

        /// <summary>
        /// If this poll is being retreived from the database, please make a call to LOAD it.
        /// This will populate the actual values for channel/message.
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public async Task LoadEntity(IDiscordClient Client)
        {
            this.channel = await Client.GetChannelAsync(this.ChannelId, CacheMode.AllowDownload) as ITextChannel;
            this.message = await channel.GetMessageAsync(this.MessageId, CacheMode.AllowDownload) as IUserMessage;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; private set; }

        /// <summary>
        /// The question being asked... Or poll topic.
        /// </summary>
        [Required]
        public string Question { get; }

        #region Channel and Message fields/properties.
        /// <summary>
        /// The Channel ID where the poll is occuring.
        /// </summary>
        [Required]
        public ulong ChannelId { get; private set; }

        /// <summary>
        /// The specific messageID for the poll.
        /// </summary>
        [Required]
        public ulong MessageId { get; set; }

        /// <summary>
        /// The Channel Object, where the poll is occuring.
        /// </summary>
        [NotMapped]
        public IMessageChannel Channel
        {
            get => channel;
            set
            {
                this.channel = value;
                this.ChannelId = value.Id;
            }

        }

        [NotMapped]
        /// <summary>
        /// The specific message, which holds the poll.
        /// </summary>
        public IUserMessage Message
        {
            get => message;
            set
            {
                this.message = value;
                this.MessageId = value.Id;
            }
        }


        [NotMapped]
        private IMessageChannel channel;

        [NotMapped]
        private IUserMessage message;

        #endregion
        /// <summary>
        /// The poll options.
        /// </summary>
        public virtual ICollection<PollOption> Options { get; } = new List<PollOption>();

        /// <summary>
        /// User votes for this poll.
        /// </summary>
        public virtual ICollection<UserVote> Votes { get; } = new List<UserVote>();


        public IEnumerable<(IEmote Emote, string OptionName, int Votes)> GetResults()
        {
            return this.Options
                 .Select(opt => (Emote: opt.Emote, OptionName: opt.Name, Votes: Votes.Where(o => o.Option == opt).Count()))
                 .Where(o => o.Votes > 0)
                 .OrderByDescending(o => o.Votes);
        }
    }
}

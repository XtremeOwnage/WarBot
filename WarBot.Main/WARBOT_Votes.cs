using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;
using Ninject;
using WarBot.Core.Voting;
using WarBot.Storage;
using System.Collections.Generic;
using System.Text;

namespace WarBot
{
    public partial class WARBOT
    {
        /// <summary>
        /// Dialogs, will force a specific user/channel combination, into a "stateful" conversation with the bot.
        /// Examples: A bot setup dialog, Data entry, etc.
        /// While a user is inside of a dialog, other message handlers will be bypassed.
        /// </summary>


        //Keep track of active polls.
        //ulong = messageID, Poll = Poll results.
        private ConcurrentDictionary<ulong, Poll> ActivePolls { get; } = new ConcurrentDictionary<ulong, Poll>();

        public Task AddPoll(Poll Poll, TimeSpan TriggerWhen)
        {

            this.ActivePolls.TryAdd(Poll.MessageId, Poll);

            Poll.Start(TriggerWhen);
            //Schedule a job to end the poll.
            Jobs.Schedule<IWARBOT>(o => o.EndPoll(Poll.MessageId), TriggerWhen);

            return Task.CompletedTask;
        }

        public async Task EndPoll(ulong MessageId)
        {
            //Remove the poll from active polls.
            if (ActivePolls.ContainsKey(MessageId) && ActivePolls.TryRemove(MessageId, out var poll))
            {
                //Make the poll as inactive.
                poll.End();

                var Channel = poll.Channel;
                var Message = poll.Message;

                //Clear all votes, before recount.
                poll.Votes.Clear();

                //Create dictionary of option to votes.
                Dictionary<PollOption, int> Results = new Dictionary<PollOption, int>();

                //Recount all votes, and store to a dictionary.
                foreach (var opt in poll.Options)
                {
                    //ToDo - Add Logic to ensure a user did not vote twice.
                    var votes = await Message.GetReactionUsersAsync(opt.Emote, 9000).FlattenAsync();
                    //Ignore bots.
                    Results.Add(opt, votes.Where(o => o.IsBot == false).Count());
                }
                var sb = new StringBuilder()
                    .AppendLine($"POLL RESULTS: {poll.Question}");
                foreach (var o in Results)
                    sb.AppendLine($"{o.Value} = {o.Key.Name}");

                await Channel.SendMessageAsync(sb.ToString());
            }
        }


        private async Task Client_MessageDeleted_Poll(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (!ActivePolls.ContainsKey(arg1.Id))
                return;

            //Delete the poll for the list of active polls.
            ActivePolls.TryRemove(arg1.Id, out _);

            //Send a message, regarding the poll being removed.
            await arg2.SendMessageAsync("The poll was deleted.");
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            try
            {
                //Check to see if the message was apart of a poll. If not, return.
                if (!ActivePolls.ContainsKey(arg1.Id))
                    return;
                //Ignore if this was a reaction I added.
                else if (arg3.UserId == Client.CurrentUser.Id)
                    return;

                var CurrentPoll = ActivePolls[arg1.Id];

                if (CurrentPoll.Options.Any(o => o.Emote == arg3.Emote))
                {
                    await arg2.SendMessageAsync($"Note- One of the options for the pool was removed by {arg3.User.Value.Mention}. I have added the reaction back.");

                    //Re-add the reaction back into the message.
                    await CurrentPoll.Message.AddReactionAsync(arg3.Emote);
                }
                else
                {
                    //The reaction removed was not apart of the poll.
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            //Todo - efficiency improvements. Goes slow. Might be discord.net's problem.
            try
            {
                if (!ActivePolls.ContainsKey(arg1.Id))
                    return;
                //Ignore if I(Warbot) added the reaction.
                else if (arg3.UserId == Client.CurrentUser.Id)
                    return;

                var CurrentPoll = ActivePolls[arg1.Id];

                //Check if this emote is supposed to be apart of the poll.
                if (CurrentPoll.Options.FirstOrDefault(o => o.Emote.Equals(arg3.Emote)).IsNotNull(out PollOption option))
                {
                    //Test if the user has already voted.
                    if (CurrentPoll.Votes.Where(o => o.UserId == arg3.UserId).IsNotNullOrEmpty(out var DuplicateVotes))
                    {
                        //Remove their other reactions.
                        foreach (var rtd in DuplicateVotes)
                            await CurrentPoll.Message.RemoveReactionAsync(rtd.Option.Emote, arg3.User.Value);
                    }

                    //Update their vote.
                    CurrentPoll.Votes.Add(new UserVote(arg3.User.Value, option));
                }
                else //This emote was NOT apart of the poll. Remove it.
                {
                    await CurrentPoll.Message.RemoveReactionAsync(arg3.Emote, arg3.User.Value);

                    var dm = await arg3.User.Value.GetOrCreateDMChannelAsync();
                    await dm.SendMessageAsync($"{arg3.User.Value.Mention}, Please do not add new reactions to my poll.");

                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }
    }
}

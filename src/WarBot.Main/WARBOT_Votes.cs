using System.Collections.Concurrent;
using WarBot.Core.Voting;
using WarBot.Modules;

namespace WarBot
{
    public partial class WARBOT : IWarBOT
    {
        /// <summary>
        /// Dialogs, will force a specific user/channel combination, into a "stateful" conversation with the bot.
        /// Examples: A bot setup dialog, Data entry, etc.
        /// While a user is inside of a dialog, other message handlers will be bypassed.
        /// </summary>


        //Keep track of active polls.
        //ulong = messageID, Poll = Poll results.
        private ConcurrentDictionary<ulong, Poll> ActivePolls { get; } = new ConcurrentDictionary<ulong, Poll>();

        //Holds a list of pending votes for a poll.
        private ConcurrentQueue<(ulong MessageId, ulong UserId, IEmote Emote, bool Add)> ReactionQueue = new ConcurrentQueue<(ulong MessageId, ulong UserId, IEmote Emote, bool Add)>();

        private readonly object PollLock = new object();

        #region Discord.net events
        //Enqueues the data to a queue to prevent from blocking the api.
        private Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            ReactionQueue.Enqueue((arg3.MessageId, arg3.UserId, arg3.Emote, false));
            return Task.CompletedTask;
        }
        //Enqueues the data to a queue to prevent from blocking the api.
        private Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            ReactionQueue.Enqueue((arg3.MessageId, arg3.UserId, arg3.Emote, true));
            return Task.CompletedTask;
        }
        /// <summary>
        /// If the message deleted was apart of a poll, we need to cancel the poll.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        private Task Client_MessageDeleted_Poll(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            return Task.Run(async () =>
            {
                if (!ActivePolls.ContainsKey(arg1.Id))
                    return;

                //Delete the poll for the list of active polls.
                ActivePolls.TryRemove(arg1.Id, out _);

                //Send a message, regarding the poll being removed.
                await arg2.SendMessageAsync("The poll was deleted.");
            });
        }
        #endregion

        #region WARBOT Add/End Poll
        public void AddPoll(Poll Poll, TimeSpan TriggerWhen)
        {
            //var sb = new StringBuilder()
            //           .AppendLine($"POLL: {Poll.Question}");
            //foreach (var o in Poll.Options)
            //    sb.AppendLine($"{o.Emote} = {o.Name}");

            //var M = Poll.Channel.SendMessageAsync(sb.ToString()).Result;
            //Poll.Message = M;

            //foreach (var o in Poll.Options)
            //    M.AddReactionAsync(o.Emote).Wait();

            //Poll.Channel.SendMessageAsync($"This poll will be automatically closed in {TriggerWhen.Humanize()}.").Wait();

            ////Add the poll to stateful DB storage.
            //var db = kernel.Get<WarDB>();
            //var x = db.Polls.Add(Poll);
            //db.SaveChanges();

            //this.ActivePolls.TryAdd(Poll.MessageId, x.Entity);

            //Poll.Start(TriggerWhen);

            ////Schedule a job to end the poll.
            //Jobs.Schedule<WARBOT>(o => o.EndPoll(Poll.MessageId), TriggerWhen);
        }

        public void EndPoll(ulong MessageId)
        {
            ////Remove the poll from active polls.
            //if (ActivePolls.ContainsKey(MessageId) && ActivePolls.TryRemove(MessageId, out var poll))
            //{
            //    //Make the poll as inactive.
            //    poll.End();

            //    var Channel = poll.Channel;
            //    var Message = poll.Message;

            //    //Process all reaction requests before ending the poll.
            //    ProcessReactions();

            //    //Create dictionary of option to votes.
            //    Dictionary<PollOption, int> Results = new Dictionary<PollOption, int>();



            //    //Recount all votes, and store to a dictionary.
            //    foreach (var opt in poll.Options)
            //    {
            //        //ToDo - Add Logic to ensure a user did not vote twice.
            //        var votes = Message.GetReactionUsersAsync(opt.Emote, 9000).FlattenAsync().Result;

            //        //Ignore bots.
            //        Results.Add(opt, votes.Where(o => o.IsBot == false).Count());
            //    }
            //    var sb = new StringBuilder()
            //        .AppendLine($"POLL RESULTS: {poll.Question}");
            //    foreach (var o in Results.Where(o => o.Value > 0).OrderByDescending(o => o.Value))
            //        sb.AppendLine($"{o.Value} = {o.Key.Name}");

            //    Channel.SendMessageAsync(sb.ToString()).Wait();
            //}

            ////remove the poll from stateful db storage.
            //var db = kernel.Get<WarDB>();
            //foreach (var td in db.Polls.Where(o => o.MessageId == MessageId).ToList())
            //    db.Polls.Remove(td);
            //db.SaveChanges();
        }

        public void ProcessReactions()
        {
            lock (PollLock)
            {

                while (ReactionQueue.TryDequeue(out var res))
                {
                    //If this message is not apart of an active poll, goto the next record..
                    if (!ActivePolls.ContainsKey(res.MessageId))
                        continue;
                    //Else if, the user who submitted the reaction was me (the bot), ignore
                    else if (res.UserId == client.CurrentUser.Id)
                        continue;

                    try
                    {
                        var CurrentPoll = ActivePolls[res.MessageId];
                        var CurrentOption = CurrentPoll.Options.FirstOrDefault(o => o.Emote.Equals(res.Emote));
                        var CurrentUser = client.GetUser(res.UserId);
                        //Check if this emote is supposed to be apart of the poll.
                        if (CurrentOption != null)
                        {
                            //User is voting.
                            if (res.Add)
                            {
                                //Remove the duplicate reactions one at a time.
                                foreach (var rtd in CurrentPoll.Votes.Where(o => o.UserId == res.UserId).ToArray())
                                {
                                    //Remove the vote from the storage.
                                    CurrentPoll.Votes.Remove(rtd);

                                    //Wait for the reaction to be removed.                                    
                                    CurrentPoll.Message.RemoveReactionAsync(rtd.Option.Emote, rtd.User).Wait();
                                }

                                //Update their vote.
                                CurrentPoll.Votes.Add(new UserVote(CurrentUser, CurrentOption));
                            }
                            else //User is removing a vote.
                            {
                                foreach (var voteToRemove in CurrentPoll.Votes.Where(o => o.Option == CurrentOption && o.UserId == CurrentUser.Id).ToArray())
                                    CurrentPoll.Votes.Remove(voteToRemove);
                            }

                        }
                        else if (res.Add)
                        {
                            //This emote was NOT apart of the poll. Remove it.
                            CurrentPoll.Message.RemoveReactionAsync(res.Emote, CurrentUser).Wait();
                        }
                        else
                        {
                            //They removed a reaction which was not apart of this poll. 
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                }
            }
        }
        #endregion

    }
}

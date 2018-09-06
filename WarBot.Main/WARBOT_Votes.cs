using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using System.Linq;
using WarBot.Core.Voting;
using WarBot.Modules.GuildCommandModules;
using Discord;
using Discord.WebSocket;
using WarBot.Core;

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
            Jobs.Schedule<IWARBOT>(o => o.EndPoll(Poll.MessageId), TriggerWhen);
            return Task.CompletedTask;
        }

        public async Task EndPoll(ulong MessageId)
        {
            if (ActivePolls.ContainsKey(MessageId) && ActivePolls.TryRemove(MessageId, out var poll))
            {
                await poll.End();
            }
        }


        private async Task Client_MessageUpdated_Poll(Cacheable<IMessage, ulong> arg1, SocketMessage arg2, ISocketMessageChannel arg3)
        {
            if (!ActivePolls.ContainsKey(arg1.Id))
                return;
            //ToDo - Check if this message affects the polls.

        }

        private async Task Client_MessageDeleted_Poll(Cacheable<IMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            if (!ActivePolls.ContainsKey(arg1.Id))
                return;

            await arg2.SendMessageAsync("The poll was deleted.");

            ActivePolls.TryRemove(arg1.Id, out _);
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            try
            {
                if (!ActivePolls.ContainsKey(arg1.Id))
                    return;
                //Ignore if this was a reaction I added.
                else if (arg3.UserId != Client.CurrentUser.Id)
                    return;

                var CurrentPoll = ActivePolls[arg1.Id];

                if (CurrentPoll.EmojiOption.ContainsKey(arg3.Emote))
                {
                    await arg2.SendMessageAsync($"Note- One of the options for the pool was removed by {arg3.User.Value.Mention}. I have added the reaction back.");
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
            try
            {
                if (!ActivePolls.ContainsKey(arg1.Id))
                    return;
                //Ignore if I(Warbot) added the reaction.
                else if (arg3.UserId == Client.CurrentUser.Id)
                    return;

                var CurrentPoll = ActivePolls[arg1.Id];
                if (CurrentPoll.EmojiOption.ContainsKey(arg3.Emote))
                {
                    //Test if the user has already voted.
                    if (CurrentPoll.UserVotes.ContainsKey(arg3.UserId))
                    {
                        var ToRemove = CurrentPoll.UserVotes[arg3.UserId];
                        //Remove the reaction.
                        await CurrentPoll.Message.RemoveReactionAsync(ToRemove, arg3.User.Value);

                        //Update their vote.
                        CurrentPoll.UserVotes[arg3.UserId] = arg3.Emote;
                    }
                    else
                    {
                        //Somebody is voting in the poll.
                        CurrentPoll.UserVotes.TryAdd(arg3.UserId, arg3.Emote);
                    }

                }
                else
                {
                    //Somebody added a new reaction.
                    await arg2.SendMessageAsync($"{arg3.User.Value.Mention}, Please do not add new reactions to my poll.");
                    await CurrentPoll.Message.RemoveReactionAsync(arg3.Emote, arg3.User.Value);
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }
    }
}

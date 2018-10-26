using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
using WarBot.Core.Voting;

namespace WarBot.Modules.GuildCommandModules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class VotingModule : GuildCommandModuleBase
    {
        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} (TimeSpan) Your question here")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote(TimeSpan HowLong, [Remainder]string Question)
        {
            if (String.IsNullOrEmpty(Question))
                await bot.OpenDialog(new PollQuestionEntryDialog(this.Context, HowLong));
            else
                await bot.OpenDialog(new PollQuestionEntryDialog(this.Context, Question, HowLong));
        }

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} (timespan)")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoTime(TimeSpan Howlong) => await StartVote(Howlong, null);

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} Your question here")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoTime([Remainder]string Question) => await StartVote(TimeSpan.FromMinutes(5), Question);

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command}")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoParams() => await StartVote_NoTime(null);
    }

    public class PollQuestionEntryDialog : WarBot.Core.Dialogs.SocketGuildDialogContextBase
    {
        //ToDo - Add more emotes.
        IEmote[] Emotes = new IEmote[] {
            new Emoji("0\u20e3"),
            new Emoji("1\u20e3"),
            new Emoji("2\u20e3"),
            new Emoji("3\u20e3"),
            new Emoji("4\u20e3"),
            new Emoji("5\u20e3"),
            new Emoji("6\u20e3"),
            new Emoji("7\u20e3"),
            new Emoji("8\u20e3"),
            new Emoji("9\u20e3"),
            };


        private State currState;
        private Poll Poll;
        private TimeSpan duration;
        private List<IMessage> cleanupMessages = new List<IMessage>();

        public PollQuestionEntryDialog(GuildCommandContext context, string question, TimeSpan Duration) : base(context)
        {
            this.Poll = new Poll(context.Channel, question);
            this.duration = Duration;
            startStep(State.GET_OPTIONS).Wait();
        }
        public PollQuestionEntryDialog(GuildCommandContext context, TimeSpan Duration) : base(context)
        {
            this.duration = Duration;
            startStep(State.GET_QUESTION).Wait();
        }

        private async Task startStep(State step)
        {
            currState = step;
            switch (step)
            {
                case State.GET_QUESTION:
                    cleanupMessages.Add(Channel.SendMessageAsync("Please let me know what the poll's topic is.").Result);
                    break;
                case State.GET_OPTIONS:
                    cleanupMessages.Add(Channel.SendMessageAsync("Please enter the poll options one at a time. Type 'Done' when you are finished." +
                      "\r\nIf this was accidental, type 'stop' to abort." +
                      "\r\nYou may also type 'remove' or 'undo' to remove the last option added.").Result);
                    break;
                case State.DONE:
                    Bot.AddPoll(Poll, this.duration);
                    goto case State.CLEANUP;
                case State.CANCELLED:
                    await Channel.SendMessageAsync($"Poll creation cancelled.");
                    goto case State.CLEANUP;

                case State.CLEANUP:
                    //Cleanup the messages from this poll.
                    await Channel.DeleteMessagesAsync(cleanupMessages);
                    await Bot.CloseDialog(this);
                    break;
            }

        }
        public override async Task ProcessMessage(SocketUserMessage input)
        {
            var msg = input.Content;
            var cmd = input.Content.Trim().ToLowerInvariant();

            //Delete this message after the dialog is over.
            cleanupMessages.Add(input);

            if (string.IsNullOrEmpty(msg))
                return;

            if (cmd.Equals("stop") || cmd.Equals("cancel"))
            {
                await startStep(State.CANCELLED);
                return;
            }
            else if (currState == State.GET_QUESTION)
            {
                this.Poll = new Poll(Channel, msg);
                cleanupMessages.Add(Channel.SendMessageAsync($"The topic has been set to:\r\n**{msg}**").Result);
                await startStep(State.GET_OPTIONS);
            }
            else if (currState != State.GET_OPTIONS)
                throw new Exception("This shouldn't be possible? Something broke.");
            else if ((cmd.Equals("remove") || cmd.Equals("undo")))
            {
                //Cleanup the message.
                if (Poll.Options.Any())
                {
                    Poll.Options.Remove(Poll.Options.Last());
                    cleanupMessages.Add(Channel.SendMessageAsync($"Item Removed.").Result);
                    return;
                }
                else
                    cleanupMessages.Add(Channel.SendMessageAsync("There were no questions in the list.").Result);
            }
            else if (msg.Equals("done"))
            {
                await startStep(State.DONE);
            }
            else //Add new option.
            {
                if (Poll.Options.Count >= Emotes.Length)
                {
                    await Channel.SendMessageAsync($"You may only add up to {Emotes.Length} options.");
                }
                else
                {
                    //Get a list of emotes which are currently used.
                    var usedEmotes = Poll.Options.Select(o => o.Emote);
                    //Find the first emote, which is not currently in use.
                    var nextReaction = this.Emotes.FirstOrDefault(o => !usedEmotes.Contains(o));
                    Poll.Options.Add(new PollOption(msg, nextReaction));
                    return;
                }
            }
        }

        private enum State
        {
            GET_QUESTION = 0,
            GET_OPTIONS = 1,

            //Poll data entry is completed.
            DONE = 2,

            //Poll was cancelled.
            CANCELLED = 3,

            //Cleanup step.
            CLEANUP = 4,
        }
    }
}
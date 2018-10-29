using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core.ModuleType;
using WarBot.Core.Voting;

namespace WarBot.Modules.Dialogs
{
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
                    await SendAsync("Please let me know what the poll's topic is.");
                    break;
                case State.GET_OPTIONS:
                    await SendAsync("Please enter the poll options one at a time. Type 'Done' when you are finished." +
                      "\r\nIf this was accidental, type 'stop' to abort." +
                      "\r\nYou may also type 'remove' or 'undo' to remove the last option added.");
                    break;
                case State.DONE:
                    Bot.AddPoll(Poll, this.duration);
                    goto case State.CLEANUP;
                case State.CANCELLED:
                    await Channel.SendMessageAsync($"Poll creation cancelled.");
                    goto case State.CLEANUP;

                case State.CLEANUP:
                    await Bot.CloseDialog(this);
                    break;
            }

        }
        public override async Task ProcessMessage(SocketUserMessage input)
        {
            var msg = input.Content;
            var cmd = input.Content.Trim().ToLowerInvariant();

            //Delete this message after the dialog is over.
            CleanupList.Add(input);

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
                await SendAsync($"The topic has been set to:\r\n**{msg}**");
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
                    await SendAsync($"Item Removed.");
                    return;
                }
                else
                    await SendAsync("There were no questions in the list.");
            }
            else if (cmd.Equals("done"))
            {
                if (Poll.Options.Count == 0)
                    await SendAsync("You have not added any options to the poll. Please provide an option.");
                else if (Poll.Options.Count < 2)
                    await SendAsync("You have not added enough options to the poll. You must provide at least two options.");
                else
                    await startStep(State.DONE);
            }
            else //Add new option.
            {
                if (Poll.Options.Count >= Emotes.Length)
                {
                    await SendAsync($"You may only add up to {Emotes.Length} options.");
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

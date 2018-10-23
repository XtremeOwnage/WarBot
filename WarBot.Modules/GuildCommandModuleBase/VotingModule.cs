using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Humanizer;
using System;
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
        [Command("start vote"), Alias("vote", "new vote")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} (TimeSpan) Your question here")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote(TimeSpan HowLong, [Remainder]string Question)
        {
            if (String.IsNullOrEmpty(Question))
            {
                await ReplyAsync($"You must ask a question.");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Please enter the poll options one at a time. Type 'Done' when you are finished." +
                              "\r\nIf this was accidental, type 'stop' to abort." +
                              "\r\nYou may also type 'remove' or 'undo' to remove the last option added.");

                await bot.OpenDialog(new PollQuestionEntryDialog(this.Context, Question, HowLong));
            }
        }
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


        private Poll Poll;
        private TimeSpan duration;

        public PollQuestionEntryDialog(GuildCommandContext context, string question, TimeSpan Duration) : base(context)
        {
            this.Poll = new Poll(context.Channel, question);
            this.duration = Duration;
        }
        public override async Task ProcessMessage(SocketUserMessage input)
        {
            var msg = input.Content;

            if (string.IsNullOrEmpty(msg))
                return;
            else if (msg.Equals("remove", System.StringComparison.OrdinalIgnoreCase) || msg.Equals("undo", System.StringComparison.OrdinalIgnoreCase))
            {
                if (Poll.Options.Any())
                {
                    Poll.Options.Remove(Poll.Options.Last());
                    await Channel.SendMessageAsync($"Item Removed.");
                    return;
                }
                else
                {
                    await Channel.SendMessageAsync("There were no questions in the list.");
                    return;
                }
            }
            else if (msg.Equals("stop", System.StringComparison.OrdinalIgnoreCase))
            {
                await Channel.SendMessageAsync($"Poll creation cancelled.");
                await Bot.CloseDialog(this);
                return;
            }
            else if (msg.Equals("done", System.StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var sb = new StringBuilder()
                        .AppendLine($"POLL: {Poll.Question}");
                    foreach (var o in Poll.Options)
                        sb.AppendLine($"{o.Emote} = {o.Name}");

                    var M = await Channel.SendMessageAsync(sb.ToString());
                    Poll.Message = M;

                    foreach (var o in Poll.Options)
                        await M.AddReactionAsync(o.Emote);

                    await Channel.SendMessageAsync($"This poll will be automatically closed in {duration.Humanize()}.");

                    await Bot.AddPoll(Poll, this.duration);

                    await Bot.CloseDialog(this);
                    return;
                }
                catch (Exception ex)
                {
                    await Channel.SendMessageAsync(ex.Message);
                }
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
    }
}
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Core.ModuleType;
using WarBot.Storage.Models.Voting;

namespace WarBot.Modules.Dialogs
{
    public class PollOptionEntryDialog : Core.Dialogs.SocketDialogContextBase
    {
        //ToDo - Add more emotes.
        List<IEmote> Emotes = new List<IEmote> {
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


        Poll Poll;
        TimeSpan duration;

        public PollOptionEntryDialog(CommandContext context, string question, TimeSpan Duration)
            : base(context)
        {
            this.Poll = new Poll(context.Channel, question);
            this.duration = Duration;
        }
        public override async Task OnCreated()
        {
            await this.Channel.SendMessageAsync("Please enter the poll options one at a time. Type 'Done' when you are finished." +
               "\r\nIf this was accidental, type 'stop' to abort." +
               "\r\nYou may also type 'remove' or 'undo' to remove the last option added.");
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
                    var lastOpt = Poll.Options.Last();

                    //Re-add the emote to the list of available emotes.
                    Emotes.Insert(0, lastOpt.Emote);

                    Poll.Options.Remove(lastOpt);
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

                    //Pin the message, if we have permissions.
                    //ToDo - Add an actual permissions check.
                    try
                    {
                        await M.PinAsync();
                    }
                    catch
                    {

                    }

                    foreach (var o in Poll.Options)
                        await M.AddReactionAsync(o.Emote);

                    await Bot.AddPoll(Poll, this.duration);

                    //Close this dialog.
                    await CloseDialog();
                }
                catch (Exception ex)
                {
                    await Channel.SendMessageAsync(ex.Message);
                }
            }
            else //Add new option.
            {
                if (Emotes.Count == 0)
                {
                    await Channel.SendMessageAsync($"You may not add any more options.");
                }
                else
                {
                    var NextEmote = Emotes.First();
                    Poll.Options.Add(new PollOption(msg, NextEmote));
                    Emotes.Remove(NextEmote);
                }
            }
        }
    }
}

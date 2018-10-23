using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.CommandModuleBase
{
    /// <summary>
    /// Random commands... with little to no logic.
    /// </summary>

    public class RandomCommandsModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [Command("thanks")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Thanks()
        {
            string[] Messages = new string[]
            {
                "Your welcome",
                "Anytime!",
                "Glad I could be of assistance.",
                "I am happy to be helpful.",
            };
            //Pick a random message from the list above, and say it.
            int num = new Random().Next(0, Messages.Length);
            await ReplyAsync(Messages[num]);
        }

        [Command("you suck"), Alias("your fired", "you're fired")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task BAD()
        {
            string[] Messages = new string[]
            {
                "Sorry :-(",
                "I tried...",
                "Are you sure its not due to user error?",
                "I did performed what I was programmed to do!",
            };

            int num = new Random().Next(0, Messages.Length);
            await ReplyAsync(Messages[num]);
            await ReplyAsync("If I have broken functionality, or errors, Please report it at https://github.com/XtremeOwnage/WarBot");
        }

        [Command("who created you")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task WhoIsAwesome()
        {
            await ReplyAsync("I was created by <@381654208073433091>.");
        }


    }
}

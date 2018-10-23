using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.Dialogs
{
    public class MimicMeDialog : SocketDialogContextBase
    {
        public MimicMeDialog(CommandContext Context)
            : base(Context) { }

        public async override Task ProcessMessage(SocketUserMessage input)
        {
            if (input.Content.Equals("stop", StringComparison.OrdinalIgnoreCase))
            {
                await this.Bot.CloseDialog(this);
                return;
            }
            //Mimic the user.
            await input.Channel.SendMessageAsync(input.Content);
        }

        public async override Task OnCreated()
        {
            await this.Channel.SendMessageAsync("Until you say 'stop', I will mimic your messages");
        }
        public async override Task OnClosed()
        {
            await this.Channel.SendMessageAsync("I have stopped.");
        }
    }
}

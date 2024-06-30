namespace WarBot.Modules.MenuComponents
{
    public class YesNoSkipStopButton : ChannelComponentBase
    {
        private ComponentBuilder builder;
        private Func<YesNoSkipStopButton, SocketMessageComponent, Task>? YesAsync;
        private Func<YesNoSkipStopButton, SocketMessageComponent, Task>? NoAsync;
        private Func<YesNoSkipStopButton, SocketMessageComponent, Task>? SkipAsync;
        private Func<YesNoSkipStopButton, SocketMessageComponent, Task>? StopAsync;

        private string yesID = Guid.NewGuid().ToString();
        private string noID = Guid.NewGuid().ToString();
        private string skipID = Guid.NewGuid().ToString();
        private string stopID = Guid.NewGuid().ToString();

        public static async Task<YesNoSkipStopButton> Build(ITextChannel Channel
            , string Prompt
            , Func<YesNoSkipStopButton, SocketMessageComponent, Task>? YesAsync = null
            , Func<YesNoSkipStopButton, SocketMessageComponent, Task>? NoAsync = null
            , Func<YesNoSkipStopButton, SocketMessageComponent, Task>? SkipAsync = null
            , Func<YesNoSkipStopButton, SocketMessageComponent, Task>? StopAsync = null
            , string YesButtonText = "Yes"
            , string NoButtonText = "No"
            , string SkipButtonText = "Skip"
            , string StopButtonText = "Stop"
            )
        {
            var sel = new YesNoSkipStopButton()
            {
                YesAsync = YesAsync,
                NoAsync = NoAsync,
                SkipAsync = SkipAsync,
                StopAsync = StopAsync,
            };

            sel.builder = new ComponentBuilder();
            if (YesAsync is not null)
                sel.builder.WithButton(YesButtonText, sel.yesID, ButtonStyle.Success);
            if (NoAsync is not null)
                sel.builder.WithButton(NoButtonText, sel.noID, ButtonStyle.Primary);
            if (SkipAsync is not null)
                sel.builder.WithButton(SkipButtonText, sel.skipID, ButtonStyle.Secondary);
            if (StopAsync is not null)
                sel.builder.WithButton(StopButtonText, sel.stopID, ButtonStyle.Danger);

            sel.Message = await Channel.SendMessageAsync(Prompt, components: sel.builder.Build());

            return sel;
        }

        /// <summary>
        /// Disables this Menu.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override Task Disable(SocketMessageComponent arg)
        {
            //foreach(var btn in builder.ActionRows[0].Components.OfType<ButtonComponent>())
            //{
            //    btn.IsDisabled = true;
            //}



            //Not Supported
            return Task.CompletedTask;
        }


        public override async Task<bool> ProcessMessage(SocketMessageComponent msg)
        {
            var cusID = msg.Data.CustomId;

            if (cusID == yesID && YesAsync is not null)
                await YesAsync(this, msg);
            else if (cusID == noID && NoAsync is not null)
                await NoAsync(this, msg);
            else if (cusID == skipID && SkipAsync is not null)
                await SkipAsync(this, msg);
            else if (cusID == stopID && StopAsync is not null)
                await StopAsync(this, msg);
            else
                return false;
            return true;
        }

        public override bool ContainsID(string CustomID) => CustomID == yesID || CustomID == noID || CustomID == skipID || CustomID == stopID;

    }
}
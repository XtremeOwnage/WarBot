namespace WarBot.Modules.MenuComponents
{
    public class SelectChannelMenu : ChannelComponentBase
    {
        private readonly string selectID = Guid.NewGuid().ToString();
        private readonly string skipID = Guid.NewGuid().ToString();
        private readonly string stopID = Guid.NewGuid().ToString();
        private bool DisableOnExecute = false;

        private SelectMenuBuilder builder;
        private ITextChannel channel;
        private Func<SelectChannelMenuResult, Task> OnSelectChannelTask;
        private Func<SelectChannelMenu, Task>? OnSkip;
        private Func<SelectChannelMenu, Task>? OnStop;

        public class SelectChannelMenuResult : IComponentResult<ITextChannel?>
        {
            private readonly SelectChannelMenu menu;
            private readonly SocketMessageComponent args;

            public SelectChannelMenuResult(SelectChannelMenu menu, SocketMessageComponent args, bool Skipped, ITextChannel? Channel)
            {
                this.menu = menu;
                this.args = args;
                this.Skipped = Skipped;
                this.Value = Channel;
            }
            public bool Skipped { get; }

            public ITextChannel? Value { get; }

            public Task Delete() => menu.Delete();

            public Task Disable() => menu.Disable(args);
        }

        public static async Task<SelectChannelMenu> Build(ITextChannel Channel
            , string Message
            , Func<SelectChannelMenuResult, Task> OnSelectChannelAsync
            , Func<SelectChannelMenu, Task>? OnCancelAsync = null
            , Func<SelectChannelMenu, Task>? OnSkipAsync = null
            , IEnumerable<ITextChannel>? ChannelList = null
            , bool DisableAfterExecute = true
            , string? Placeholder = "Please select a channel."
            , string SkipButtonText = "Skip"
            , string CancelButtonText = "Cancel")
        {
            var sel = new SelectChannelMenu()
            {
                channel = Channel,
                OnSelectChannelTask = OnSelectChannelAsync,
                OnSkip = OnSkipAsync,
                OnStop = OnCancelAsync,
                DisableOnExecute = DisableAfterExecute,
            };

            sel.builder = new SelectMenuBuilder()
                .WithPlaceholder(Placeholder)
                .WithCustomId(sel.selectID);

            if (ChannelList is null)
                ChannelList = (await Channel.Guild.GetChannelsAsync()).OfType<ITextChannel>().Take(20).ToList();

            foreach (var ch in ChannelList)
                sel.builder.AddOption(ch.Name, ch.Id.ToString(), ch.Topic);

            var comp = new ComponentBuilder()
                .WithSelectMenu(sel.builder);

            if (OnSkipAsync is not null)
                comp = comp.WithButton(SkipButtonText, sel.skipID, ButtonStyle.Secondary, row: 1);
            if (OnCancelAsync is not null)
                comp = comp.WithButton(CancelButtonText, sel.stopID, ButtonStyle.Danger, row: 1);


            sel.Message = await Channel.SendMessageAsync(Message, components: comp.Build());

            return sel;
        }



        /// <summary>
        /// Disables this Menu.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public override async Task Disable(SocketMessageComponent arg)
        {
            builder.IsDisabled = true;
            Disabled = true;

            var selected = builder.Options.FirstOrDefault(o => o.Value == arg.Data.Values.FirstOrDefault());

            if (selected is not null)
            {
                selected.IsDefault = true;
            }

            await arg.UpdateAsync(x =>
            {
                x.Components = new ComponentBuilder().WithSelectMenu(builder).Build();
            });
        }

        public override async Task<bool> ProcessMessage(SocketMessageComponent msg)
        {
            var cusID = msg.Data.CustomId;

            if (cusID == selectID && OnSelectChannelTask is not null)
            {
                var CH = (await channel.Guild.GetChannelsAsync())
                    .OfType<ITextChannel>()
                    .FirstOrDefault(o => o.Id.ToString() == msg.Data.Values.FirstOrDefault());

                var res = new SelectChannelMenuResult(this, msg, false, CH);
                await OnSelectChannelTask.Invoke(res);
            }
            else if (cusID == skipID && OnSkip is not null)
                await OnSkip(this);
            else if (cusID == stopID && OnStop is not null)
                await OnStop(this);
            else
                return false;

            if (DisableOnExecute)
                await Disable(msg);

            return true;
        }

        public override bool ContainsID(string CustomID) => this.selectID == CustomID || this.stopID == CustomID || this.skipID == CustomID;
    }
}
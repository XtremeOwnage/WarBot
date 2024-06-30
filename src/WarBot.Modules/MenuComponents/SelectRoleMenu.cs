namespace WarBot.Modules.MenuComponents
{
    public class SelectRoleMenu : ChannelComponentBase
    {
        private readonly string selectID = Guid.NewGuid().ToString();
        private readonly string skipID = Guid.NewGuid().ToString();
        private readonly string stopID = Guid.NewGuid().ToString();
        private bool DisableOnExecute = false;

        private SelectMenuBuilder builder;
        private ITextChannel channel;
        private Func<Result, Task> OnSelectedRole;
        private Func<SelectRoleMenu, Task>? OnSkip;
        private Func<SelectRoleMenu, Task>? OnStop;

        public class Result : IComponentResult<IRole?>
        {
            private readonly SelectRoleMenu menu;
            private readonly SocketMessageComponent args;

            public Result(SelectRoleMenu menu, SocketMessageComponent args, bool Skipped, IRole? Role)
            {
                this.menu = menu;
                this.args = args;
                this.Skipped = Skipped;
                this.Value = Role;
            }
            public bool Skipped { get; }

            public IRole? Value { get; }

            public Task Delete() => menu.Delete();

            public Task Disable() => menu.Disable(args);
        }

        public static async Task<SelectRoleMenu> Build(ITextChannel Channel
            , string Message
            , Func<Result, Task> OnSelectedRoleAsync
            , Func<SelectRoleMenu, Task>? OnCancelAsync = null
            , Func<SelectRoleMenu, Task>? OnSkipAsync = null
            , IEnumerable<IRole>? RoleList = null
            , bool DisableAfterExecute = true
            , string? Placeholder = "Please select a role."
            , string SkipButtonText = "Skip"
            , string CancelButtonText = "Cancel")
        {
            var sel = new SelectRoleMenu()
            {
                channel = Channel,
                OnSelectedRole = OnSelectedRoleAsync,
                OnSkip = OnSkipAsync,
                OnStop = OnCancelAsync,
                DisableOnExecute = DisableAfterExecute,
            };

            sel.builder = new SelectMenuBuilder()
                .WithPlaceholder(Placeholder)
                .WithCustomId(sel.selectID);

            if (RoleList is null)
                RoleList = Channel.Guild.Roles.ToList();

            foreach (var ro in RoleList)
                sel.builder.AddOption(ro.Name, ro.Id.ToString());

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

            if (cusID == selectID && OnSelectedRole is not null)
            {
                var role = channel.Guild.Roles.FirstOrDefault(o => o.Id.ToString() == msg.Data.Values.FirstOrDefault());

                var res = new Result(this, msg, false, role);
                await OnSelectedRole.Invoke(res);
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
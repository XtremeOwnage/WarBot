namespace WarBot.Modules.GuildCommand;

[RequireContext(ContextType.Guild)]
public class TemplateModule : WarBOTModule
{
    [Command("command_goes_here"), Alias("add_alias")]
    [Summary("Description goes here.")]
    [CommandUsage("{prefix} command_here")]
    [RoleLevel(RoleLevel.None, RoleMatchType.LESS_THEN)]
    [RequireBotPermission(GuildPermission.ChangeNickname)]
    public async Task TemplateTask([Remainder] string Nickname)
    {
        //Make sure to remove this line.
        if (true)
            await Task.CompletedTask;

        var Me = Context.Guild.CurrentUser;
        var user = Context.User as SocketGuildUser;

        if (user == null)
            throw new NullReferenceException("User was not socket guild user.");

        if (this.cfg == null)
            throw new NullReferenceException("IGuildConfig was null");

        //If user is ME(WarBot)
        if (Context.User.Id == Me.Id)
            await RespondAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
        //Check if the user is a higher permission-level then WarBot - Will prevent warbot from managing target user.
        if (user.Hierarchy > Me.Hierarchy)
        {
            await RespondAsync($"The target user is a member of a higher role then I am. I cannot kick that user.");
            return;
        }

        if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
        {
            await RespondAsync("The provided nickname was not valid.");
            return;
        }
    }
}

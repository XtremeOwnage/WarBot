namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[RoleLevel(RoleLevel.Leader)]
public class KickBanModule : WarBOTModule
{

    [UserCommand("Kick")]
    [RequireBotPermission(GuildPermission.KickMembers)]
    public Task Kick_UserCommand(IUser user) => Kick_SlashCommand(user as SocketGuildUser ?? throw new Exception("Expected socket guild user??!?"), null);

    [SlashCommand("kick", "Kicks the specified user from the guild.")]
    [RequireBotPermission(GuildPermission.KickMembers | GuildPermission.SendMessages)]
    public async Task Kick_SlashCommand(SocketGuildUser user, [Summary("reason", "Reason for kick")] string Message = "An admin determined your services were no longer required.")
    {
        var Me = Context.Guild.CurrentUser;

        //Make sure to not kick myself.
        if (user.Id == Me.Id)
            await RespondAsync("Sorry, I do not wish to kick myself. You may ask me to /leave though.");
        //Do a permissions check.
        else if (user.Hierarchy > Me.Hierarchy)
        {
            await RespondAsync($"The target user is a member of a higher role then I am. I cannot kick that user.");
        }
        //Kick the user.
        else
        {
            await user.KickAsync(Message);
            var eb = new EmbedBuilder()
                .WithTitle("User Kicked")
                .WithColor(Color.Red)
                .WithDescription($"User {user.Mention} has been removed from this guild by {Context.User.Mention}.")
                .AddField("Reason", Message);

            await RespondAsync(embed: eb.Build());
        }
    }

    [UserCommand("Ban")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public Task Ban_UserCommand(IUser user) => Ban_SlashCommand(user as SocketGuildUser ?? throw new Exception("Expected socket guild user??!?"));

    [SlashCommand("ban", "Remove a user from this guild.")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task Ban_SlashCommand(SocketGuildUser user,
        [Summary("reason", "Reason to provide for kick")] string Message = "An admin determined your services were no longer required.")
    {
        var Me = Context.Guild.CurrentUser;

        if (user.Id == Me.Id)
            await RespondAsync("Sorry, I do not wish to kick myself. You may ask me to leave though.");
        //Do a permissions check.
        else if (user.Hierarchy > Me.Hierarchy)
        {
            await RespondAsync($"The target user is a member of a higher role then I am. I cannot ban that user.");
        }
        else
        {
            await user.BanAsync(reason: Message);
            var eb = new EmbedBuilder()
                .WithTitle("User Banned")
                .WithColor(Color.Red)
                .WithDescription($"User {user.Mention} has been banned from this guild by {Context.User.Mention}.")
                .AddField("Reason", Message);

            await RespondAsync(embed: eb.Build());
        }
    }

    [UserCommand("Mute - 1 Hour")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public Task Mute_UserCommand(IUser user) => Mute_SlashCommand(user as SocketGuildUser ?? throw new Exception("Expected socket guild user??!?"), Hours: 1, Message: "Context action");

    [SlashCommand("mute", "Places a user in timeout.")]
    [RequireBotPermission(GuildPermission.BanMembers)]
    public async Task Mute_SlashCommand(SocketGuildUser user,
        [Summary("hours", "How many hours to place user in timeout.")] int Hours = 1,
        [Summary("reason", "Reason to provide for mute/timeout.")] string Message = "No reason provided")
    {
        var Me = Context.Guild.CurrentUser;

        if (user.Id == Me.Id)
            await RespondAsync("Sorry, I do not wish to mute myself. You may ask me to leave though.");
        //Do a permissions check.
        else if (user.Hierarchy > Me.Hierarchy)
        {
            await RespondAsync($"The target user is a member of a higher role then I am. I cannot manage that user.");
        }
        else
        {
            await user.SetTimeOutAsync(TimeSpan.FromHours(Hours));
            var eb = new EmbedBuilder()
                .WithTitle("User Muted")
                .WithColor(Color.Red)
                .WithDescription($"User {user.Mention} has been placed in timeout by {Context.User.Mention} for {Hours} hours.")
                .AddField("Reason", Message);

            await RespondAsync(embed: eb.Build());
        }
    }
}
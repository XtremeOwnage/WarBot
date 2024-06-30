namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[Group("disable", "Disable various pieces of functionality.")]
public class DisableModule : WarBOTModule
{
    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("greeting", "Disables greeting messages.")]
    public async Task Greeting()
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.Event_UserJoin.Enabled = false;
            await logic.SaveChangesAsync();
            await RespondAsync("New user greeting messages have been disabled.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("farewell", "Disables farewell messages.")]
    public async Task Farewell()
    {
        await UseGuildLogicAsync(async logic =>
        {

            logic.Event_UserLeft.Enabled = true;
            await logic.SaveChangesAsync();
            await RespondAsync("Farewell messages have been disabled.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("portal", "Disables notifications when portal opens.")]
    public async Task DisablePortal()
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.HustleSettings.Event_Portal.Enabled = false;
            await logic.SaveChangesAsync();

            await RespondAsync("I will no longer deliver portal notifications.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("updates", "Disable WarBOT update notifications.")]
    public async Task DisableUpdates()
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.Event_Updates.Enabled = false;
            await logic.SaveChangesAsync();

            await RespondAsync("I will no longer send notifications when I updated.");
        });
    }
}


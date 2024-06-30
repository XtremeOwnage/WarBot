namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[Group("reset", "Reset various piece of functionality.")]
public class ResetConfigModule : WarBOTModule
{
    [SlashCommand("config", "Reset warbot's configuration to defaults.")]
    [RoleLevel(RoleLevel.Leader, RoleMatchType.GREATER_THEN_OR_EQUAL)]
    public async Task ResetConfig()
    {
        await UseGuildLogicAsync(async logic =>
        {
            var res = await ConfirmAsync(TimeSpan.FromMinutes(1), "Are you SURE you want to reset my config? All changes will be lost and this is not reversible.");
            if (!res)
            {
                await RespondAsync("Cancelled");
                return;
            }


            logic.ClearSettings();
            await logic.SaveChangesAsync();

            await RespondAsync("All of my settings have been reverted to default. Please configure me now using /setup");
        });
    }
}
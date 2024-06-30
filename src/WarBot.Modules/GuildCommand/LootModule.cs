namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
public class LootModule : WarBOTModule
{
    [RoleLevel(RoleLevel.Guest)]
    [SlashCommand("loot", "Display this guild's loot instructions, if set.")]
    public async Task ShowLoot()
    {
        await UseGuildLogicAsync(async logic =>
        {
            if (string.IsNullOrEmpty(logic.HustleSettings.LootMessage))
            {
                await RespondAsync("Sorry, your leader has not set a value for this command yet.");
            }
            else
            {
                await RespondAsync(logic.HustleSettings.LootMessage);
            }
        });
    }
}

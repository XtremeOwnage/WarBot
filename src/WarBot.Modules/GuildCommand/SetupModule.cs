namespace WarBot.Modules.GuildCommand;
public class SetupModule : WarBOTModule
{

    [SlashCommand("setup", "Configure WarBOT to your needs")]
    [RoleLevel(RoleLevel.ServerAdmin, RoleMatchType.GREATER_THEN_OR_EQUAL)]
    public Task Setup()
    {
        return RespondAsync($"To configure me, please visit {BotConfig.PUBLIC_URL}", ephemeral: true);
    }
}


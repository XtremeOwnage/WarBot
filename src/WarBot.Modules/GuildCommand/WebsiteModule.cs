namespace WarBot.Modules.GuildCommand;

[RequireContext(ContextType.Guild)]
public class WebsiteModule : WarBOTModule
{

    [SlashCommand("website", "Display this guild's website, if set.")]
    public async Task Website()
    {
        await UseGuildLogicAsync(async logic =>
        {
            if (string.IsNullOrEmpty(logic.Website))
            {
                await RespondAsync("Sorry, your leader has not set a value for this command yet.");
            }
            else
            {
                await RespondAsync(logic.Website);
            }
        });
    }
}

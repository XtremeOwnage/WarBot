namespace WarBot.Modules.Command;

public class HelpModule : WarBOTModule
{
    [SlashCommand("help", "Links to the warbot test server.")]
    public async Task Help()
    {
        await RespondAsync("My documentation is available at https://docs.warbot.dev/", ephemeral: true);
    }
}
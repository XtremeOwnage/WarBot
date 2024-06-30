namespace WarBot.Modules.Command;
/// <summary>
/// Contains simple messages, with little to no logic.
/// </summary>

public class EchoModule : WarBOTModule
{
    [SlashCommand("say", "I will say the provided message")]
    public async Task Say([Summary("text", "The text to echo")] string echo)
    {
        // RespondAsync is a method on ModuleBase
        await RespondAsync(echo);
    }

    //[Command("mimic me"), Summary("I will repeat everything you say, until you say stop.")]
    //[RequireBotPermission(ChannelPermission.SendMessages)]
    //public async Task MimicMe()
    //{
    //    await bot.OpenDialog(new Dialogs.MimicMeDialog(Context));
    //}

    [SlashCommand("ping", "I will return a pong.")]
    public async Task Ping()
    {
        await RespondAsync("**Pong**");
    }
}


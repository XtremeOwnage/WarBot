namespace WarBot.Modules.Command;
/// <summary>
/// Random commands... with little to no logic.
/// </summary>

public class RandomCommandsModule : WarBOTModule
{
    [SlashCommand("thanks", "Display random gratitude torwards WarBOT.")]
    public async Task Thanks()
    {
        string[] Messages = new string[]
        {
                "Your welcome",
                "Anytime!",
                "Glad I could be of assistance.",
                "I am happy to be helpful.",
        };
        //Pick a random message from the list above, and say it.
        int num = new Random().Next(0, Messages.Length);
        await RespondAsync(Messages[num]);
    }

    [SlashCommand("creator", "Who created me?")]
    public async Task WhoIsAwesome()
    {
        await RespondAsync("I was created by <@381654208073433091>.");
    }

    [SlashCommand("send_nudes", "I will send you nude pictures.")]
    public async Task SendNudes(IUser user)
    {
        await RespondAsync("You First.");
    }

    [SlashCommand("time", "Show warbot's current time")]
    public async Task ShowTime()
    {
        var utcnow = DateTimeOffset.UtcNow;

        await RespondAsync($"UTC: {utcnow}, Local: {DateTimeOffset.Now}");
    }


}

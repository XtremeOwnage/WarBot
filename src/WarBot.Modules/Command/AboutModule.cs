using System.Text;

namespace WarBot.Modules.Command;

public class AboutModule : WarBOTModule
{
    [SlashCommand("about", "Show some information about me.")]
    public async Task About()
    {
        StringBuilder sb = new StringBuilder();

        sb
            .AppendLine("I am a discord bot orignally designed around the mobile game Hustle Castle.")
            .AppendLine()
            .AppendLine("I also provide functionality around role management, voting, and a few other useful discord-related actions.")
            .AppendLine("If you would like to see me in your server, or would like more information, please visit this page:")
            .AppendLine("https://github.com/XtremeOwnage/WarBot/blob/master/README.md");

        await RespondAsync(sb.ToString());
    }

    [SlashCommand("github", "Links to my github page.")]
    public async Task Github()
    {
        await RespondAsync("My github is at https://github.com/XtremeOwnage/WarBot/");
    }
}

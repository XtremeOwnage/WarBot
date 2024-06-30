using System.Diagnostics;

namespace WarBot.Modules.Command;
public class StatsModule : WarBOTModule
{
    private readonly IWarBOT warBOT;
    private readonly InteractionService svc;

    public StatsModule(IWarBOT warBOT, InteractionService svc)
    {
        this.warBOT = warBOT;
        this.svc = svc;
    }

    [SlashCommand("stats", "Display command stats related to me.")]
    public async Task Stats_GUILD()
    {
        var guilds = Context.Client.Guilds;
        var MemberCount = guilds.Sum(o => o.MemberCount);
        TimeSpan ts = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();


        var eb = new EmbedBuilder()
            .WithTitle("WarBOT Statistics")
            .AddField("Process Uptime", $"{ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds")
            .AddField("Context Commands", svc.ContextCommands.Count, true)
            .AddField("Component Commands", svc.ComponentCommands.Count, true)
            .AddField("Slash Commands", svc.SlashCommands.Count, true)
            .AddField("Loaded Modules", svc.Modules.Count, true)
            .AddField("Total Shards", Context.Client.Shards.Count, true)
            .AddField("Total Guilds", guilds.Count, true)
            .AddField("Total Users", MemberCount, true)
            .AddField("Slash Interactions", MetricHelper.SlashExecutions, true)
            .AddField("Message Interactions", MetricHelper.MessageContextExecutions, true)
            .AddField("User Interactions", MetricHelper.UserContextInteractions, true)
            .AddField("Custom Executions", MetricHelper.CustomCommandExecutions, true)
            .AddField("Interaction Failures", MetricHelper.InteractionFailures, true);

        // RespondAsync is a method on ModuleBase
        await RespondAsync(embed: eb.Build());
    }
}
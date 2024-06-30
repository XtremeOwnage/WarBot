using Hangfire;

namespace WarBot.Modules.GuildCommand;

[RoleLevel(RoleLevel.ServerAdmin)]
[RequireContext(ContextType.Guild)]
public class ServerAdminModule : WarBOTModule
{

    [SlashCommand("leave", "Warbot will leave the guild.")]
    public async Task Leave()
    {
        await DeferAsync();

        var result = await ConfirmAsync(TimeSpan.FromMinutes(5), "Are you sure you want me to leave?");

        if (!result)
        {
            await this.RespondAsync("Cancelled.", ephemeral: true);
            return;
        }

        var eb = new EmbedBuilder()
            .WithTitle("GoodBye 😭")
            .WithDescription("I am sorry I did not meet the expectations of your guild. If you wish to invite me back, you may click this embed. I will retain my configuration.")
            .WithUrl(BotConfig.DISCORD_INVITE_URL_FULL);

        await RespondAsync(embed: eb.Build());
        await Context.Guild.LeaveAsync();
    }

    [SlashCommand("reload-commands", "Reloads all commands for this guild.")]
    public async Task ReloadCommands()
    {
        BackgroundJob.Enqueue<Jobs.RegisterDiscordCommandsJob>(o => o.ExecuteForGuildAsync(this.Context.Guild.Id));
        await RespondAsync("Commands have been reloaded. Please wait a second for discord to sync.", ephemeral: true);
    }
}

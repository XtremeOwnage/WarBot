using System.Text;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[RoleLevel(RoleLevel.Leader)]
[Group("portal", "Used to set Hustle Castle Portal Settings")]
public class PortalModule : WarBOTModule
{
    [SlashCommand("channel", "Sets the specified channel in the configuration")]
    public async Task SetChannel([Summary("Channel", "The target channel")] ITextChannel Channel)
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.HustleSettings.Event_Portal.Channel.SetChannel(Channel);
            await logic.SaveChangesAsync();

            await RespondAsync($"Channel {Channel.Mention} will be used for portal notifications.");
        });
    }

    [SlashCommand("message", "Sets the specified channel in the configuration")]
    public async Task SetMessage([Summary("Message", "Mesage to set")] string Message)
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.HustleSettings.Event_Portal.Message = Message;
            await logic.SaveChangesAsync();
            await RespondAsync($"Message for portal opening has been set.");

        });
    }

    [SlashCommand("enable", "Enables War Events")]
    public async Task Enable()
    {
        await UseGuildLogicAsync(async logic =>
        {
            var channel = await logic.HustleSettings.Event_Portal.Channel.GetChannelAsync();
            switch (logic.HustleSettings.Event_Portal.Enabled)
            {
                case false when channel is null:
                    await RespondAsync("You need to first set a channel for these notifications using /portal channel");
                    return;
                case true when channel is null:
                    logic.HustleSettings.Event_Portal.Enabled = false;
                    logic.HustleSettings.Event_Portal.Channel.ClearSettings();
                    await logic.SaveChangesAsync();
                    await RespondAsync("You need to set a valid channel for these notifications using /portal channel");
                    return;
                case true:
                    await RespondAsync("Portal notifications were already enabled.");
                    return;
                case false:
                    logic.HustleSettings.Event_Portal.Enabled = true;
                    await logic.SaveChangesAsync();
                    await RespondAsync("Portal notifications are now enabled.");
                    return;
            }
        });
    }

    [SlashCommand("disable", "Disable War Events")]
    public async Task Disable()
    {
        await UseGuildLogicAsync(async logic =>
        {
            if (logic.HustleSettings.Event_Portal.Enabled == true)
            {
                logic.HustleSettings.Event_Portal.Enabled = false;
                await logic.SaveChangesAsync();
                await RespondAsync("Portal notifications are now disabled.");
            }
            else
                await RespondAsync("Portal notifications were already disabled.");
        });
    }

    [SlashCommand("clear", "Configures message clearing.")]
    public async Task Clear(
             [Summary("Clear", "How should message clearing be implemented?")] EventClearType ClearType
           , [Summary("Delay", "How many minutes to wait before clearing messages?")] int DelayMins = 15)
    {
        const string Noun = "Portal";
        if (DelayMins < 5)
        {
            await RespondAsync("Delay duration must be greater then 5 minutes.");
            return;
        }
        else if (DelayMins > TimeSpan.FromDays(1).TotalMinutes)
        {
            await RespondAsync($"Delay duration must be less than 1 day (1,440 minutes)");
            return;
        }

        await UseGuildLogicAsync(async logic =>
        {
            var evt = logic.HustleSettings.Event_Portal;

            await RespondAsync($"Changed clear method for {Noun} from {evt.ClearMethod} to {ClearType} with delay of {DelayMins} minutes");
            evt.ClearMethod = ClearType;
            evt.ClearDurationMins = DelayMins;

            await logic.SaveChangesAsync();
        });
    }
}

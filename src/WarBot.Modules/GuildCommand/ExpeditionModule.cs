using System.Text;
using WarBot.DataAccess.Logic.Events;
using WarBot.Modules.GuildCommand.Models;

namespace WarBot.Modules.GuildCommand;
[RoleLevel(RoleLevel.Leader)]
[RequireContext(ContextType.Guild)]
[Group("expedition", "Used to set Hustle Castle Expedition Settings")]
public class ExpeditionModule : WarBOTModule
{
    [SlashCommand("channel", "Sets the specified channel in the configuration")]
    public async Task SetChannel([Summary("Expedition", "Which Expedition to Manage")] Models.EventNumber Target
        , [Summary("Channel", "The target channel")] ITextChannel Channel)
    {
        await UseGuildLogicAsync(async logic =>
        {
            if (Target == EventNumber.ALL)
            {
                foreach (byte i in Enumerable.Range(1, 4))
                {
                    logic.HustleSettings.GetExpedition(i).Channel.SetChannel(Channel);
                }

                await logic.SaveChangesAsync();
                await RespondAsync($"Channel {Channel.Mention} will be used for ALL expeditions.");
                return;
            }

            var evt = logic.HustleSettings.GetExpedition((byte)Target);
            if (evt is not null)
            {
                evt.Channel.SetChannel(Channel);
                await logic.SaveChangesAsync();
                await RespondAsync($"Channel {Channel.Mention} will be used for expedition {(byte)Target}.");
            }
        });
    }

    [SlashCommand("message", "Sets the specified channel in the configuration")]
    public async Task SetMessage(
        [Summary("Expedition", "Which Expedition to Manage")] Models.EventNumber Target
        , [Summary("Event", "Which message to change")] Models.WarMessage Event
        , [Summary("Message", "Mesage to set")] string Message)
    {
        await UseGuildLogicAsync(async logic =>
        {
            void SetWar(byte WarNo)
            {
                var war = logic.HustleSettings.GetExpedition(WarNo);

                switch (Event)
                {
                    case WarMessage.Event_Prep_Starting:
                        war.Prep_Started_Message = Message;
                        break;
                    case WarMessage.Event_Prep_Ending:
                        war.Prep_Ending_Message = Message;
                        break;
                    case WarMessage.Event_Started:
                        war.Event_Started_Message = Message;
                        break;
                    default:
                        break;
                }
            }

            if (Target == EventNumber.ALL)
                foreach (byte i in Enumerable.Range(1, 4))
                    SetWar(i);
            else
                SetWar((byte)Target);

            await logic.SaveChangesAsync();
            await RespondAsync($"Message for event {Event}, expedition {Target} has been set.");

        });
    }

    [SlashCommand("enable", "Enables Expedition Events")]
    public async Task Enable([Summary("War", "Which Expedition to manage")] Models.EventNumber War)
    {
        await UseGuildLogicAsync(async logic =>
        {
            StringBuilder sb = new StringBuilder();
            async Task enableWar(byte warNo)
            {
                var war = logic.HustleSettings.GetExpedition(warNo);
                if ((await war.Channel.GetChannelAsync()) is null)
                {
                    var channel = await PromptForTextChannel("Which channel should be used?");
                    if (channel is null)
                    {
                        war.Enabled = false;
                        await RespondAsync("You cannot enable this event until a channel has been set.");
                        return;
                    }
                    war.Channel.SetChannel(channel);
                }
                else if (war.Enabled)
                {
                    sb.AppendLine($"Expedition {warNo} was already enabled.");
                }
                else
                {
                    war.Enabled = true;
                    sb.AppendLine($"Enabled Expedition {warNo}");
                }
            }

            switch (War)
            {
                case EventNumber.EVENT_1:
                case EventNumber.EVENT_2:
                case EventNumber.EVENT_3:
                case EventNumber.EVENT_4:
                    await enableWar((byte)War);
                    break;
                case EventNumber.ALL:
                    foreach (int x in Enumerable.Range(1, 4))
                        await enableWar((byte)x);
                    break;
                default:
                    await RespondAsync("Invalid selection.");
                    return;
            }

            await logic.SaveChangesAsync();
            await RespondAsync(sb.ToString());
        });
    }


    [SlashCommand("disable", "Disable Expedition Events")]
    public async Task Disable([Summary("War", "Which Expedition to manage")] Models.EventNumber War)
    {
        await UseGuildLogicAsync(async logic =>
        {
            StringBuilder sb = new StringBuilder();
            async Task enableWar(byte warNo)
            {
                var war = logic.HustleSettings.GetExpedition(warNo);
                if (war.Enabled)
                {
                    war.Enabled = false;
                    sb.AppendLine($"Expedition {warNo} has been disabled");
                }
                else
                {
                    sb.AppendLine($"Expedition {warNo} was already disabled");
                }
            }

            switch (War)
            {
                case EventNumber.EVENT_1:
                case EventNumber.EVENT_2:
                case EventNumber.EVENT_3:
                case EventNumber.EVENT_4:
                    await enableWar((byte)War);
                    break;
                case EventNumber.ALL:
                    foreach (int x in Enumerable.Range(1, 4))
                        await enableWar((byte)x);
                    break;
                default:
                    await RespondAsync("Invalid selection.");
                    return;
            }

            await logic.SaveChangesAsync();
            await RespondAsync(sb.ToString());
        });
    }

    [SlashCommand("clear", "Configures message clearing.")]
    public async Task Clear([Summary("War", "Which expedition to manage")] Models.EventNumber War
        , [Summary("Method", "How should message clearing be implemented?")] EventClearType ClearType
        , [Summary("Delay", "How many minutes to wait before clearing messages?")] int DelayMins = 15)
    {
        const string Noun = "Expedition";
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
            StringBuilder sb = new StringBuilder();
            async Task setClear(byte warNo)
            {
                var war = logic.HustleSettings.GetExpedition(warNo);
                if (war.ClearMethod != ClearType)
                {
                    sb.AppendLine($"Changed clear method for {Noun} {warNo} from {war.ClearMethod} to {ClearType}");
                    war.ClearMethod = ClearType;
                }
                if (war.ClearDurationMins != DelayMins)
                {
                    sb.AppendLine($"Changed clear delay mins for {Noun} {warNo} from {war.ClearDurationMins} to {DelayMins}");
                    war.ClearDurationMins = DelayMins;
                }
            }

            switch (War)
            {
                case EventNumber.EVENT_1:
                case EventNumber.EVENT_2:
                case EventNumber.EVENT_3:
                case EventNumber.EVENT_4:
                    await setClear((byte)War);
                    break;
                case EventNumber.ALL:
                    foreach (int x in Enumerable.Range(1, 4))
                        await setClear((byte)x);
                    break;
                default:
                    await RespondAsync("Invalid selection.");
                    return;
            }

            await logic.SaveChangesAsync();
            await RespondAsync(sb.ToString());
        });
    }
}

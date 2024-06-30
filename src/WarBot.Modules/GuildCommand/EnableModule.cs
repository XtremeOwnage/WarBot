namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[Group("enable", "Enable various pieces of functionality.")]
public class EnableModule : WarBOTModule
{
    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("greeting", "Enables greeting messages.")]
    public async Task Greeting()
    {
        await UseGuildLogicAsync(async logic =>
        {

            var ch = await logic.Event_UserJoin.Channel.GetChannelAsync();

            if (ch is not null)
            {
                logic.Event_UserJoin.Enabled = true;
                await logic.SaveChangesAsync();
                await RespondAsync("New user greeting messages have been enabled.");
            }

            var channel = await PromptForTextChannel("Where should I greet new users?");

            if (channel is null)
            {
                logic.Event_UserJoin.Enabled = false;
                await logic.SaveChangesAsync();
                await RespondAsync("User greetings cannot be enabled until you have configured a channel.");
                return;
            }

            logic.Event_UserJoin.Channel.SetChannel(channel);
            logic.Event_UserJoin.Enabled = true;
            await logic.SaveChangesAsync();
            await RespondAsync($"I greet new users in {channel.Mention}");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("farewell", "Enables farewell messages.")]
    public async Task Farewell()
    {
        await UseGuildLogicAsync(async logic =>
        {
            var ch = await logic.Event_UserLeft.Channel.GetChannelAsync();

            if (ch is not null)
            {
                logic.Event_UserLeft.Enabled = true;
                await logic.SaveChangesAsync();
                await RespondAsync("Farewell messages have been enabled.");
                return;
            }

            var channel = await PromptForTextChannel("Where should I send notifications when a user leaves?");

            if (channel is null)
            {
                logic.Event_UserLeft.Enabled = false;
                await logic.SaveChangesAsync();
                await RespondAsync("Farewall messages cannot be enabled until you have configured a channel to receive them.");
                return;
            }

            logic.Event_UserLeft.Channel.SetChannel(channel);
            logic.Event_UserLeft.Enabled = true;
            await logic.SaveChangesAsync();
            await RespondAsync($"I will send messages to {channel.Mention} when someone leaves.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("portal", "Enables notifications when portal opens.")]
    public async Task EnablePortal()
    {
        await UseGuildLogicAsync(async logic =>
        {
            var targetChannel = await logic.HustleSettings.Event_Portal.Channel.GetChannelAsync();
            if (targetChannel is not null)
            {
                logic.HustleSettings.Event_Portal.Enabled = true;
                await logic.SaveChangesAsync();
                await RespondAsync($"Portal opening events will be announced in {targetChannel.Mention}");
                return;
            }

            var channel = await PromptForTextChannel("Which channel should receive portal notifications?");

            if (channel is null)
            {
                logic.HustleSettings.Event_Portal.Enabled = false;
                await logic.SaveChangesAsync();
                await RespondAsync("Portal opened notifications cannot be enabled until you have configured a channel.");
                return;
            }

            logic.HustleSettings.Event_Portal.Channel.SetChannel(channel);
            logic.HustleSettings.Event_Portal.Enabled = true;
            await logic.SaveChangesAsync();
            await RespondAsync($"I will send portal notifications to the channel {channel.Mention}");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("updates", "Enables notifications regarding WarBOT Updates")]
    public async Task EnableUpdates()
    {
        await UseGuildLogicAsync(async logic =>
        {
            var targetChannel = await logic.Event_Updates.Channel.GetChannelAsync();
            if (targetChannel is not null)
            {
                logic.Event_Updates.Enabled = true;
                await RespondAsync($"I will send update notifications to the channel {targetChannel.Mention}");
                await logic.SaveChangesAsync();
                return;
            }
            //Prompt for a channel?
            var channel = await PromptForTextChannel("Which channel should receive update notifications?");

            if (channel is null)
            {
                logic.Event_Updates.Enabled = false;
                await logic.SaveChangesAsync();
                await RespondAsync("Update notifications cannot be enabled until you have configured a channel");
                return;
            }

            logic.Event_Updates.Channel.SetChannel(channel);
            logic.Event_Updates.Enabled = true;
            await logic.SaveChangesAsync();
            await RespondAsync($"I will send update notifications to the channel {channel.Mention}");

        });
    }

}

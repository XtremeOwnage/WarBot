using WarBot.DataAccess.Logic.Events;
using WarBot.Modules.GuildCommand.Models;

namespace WarBot.Modules.GuildCommand;
[RequireContext(ContextType.Guild)]
[Group("set", "Used to set various parameters related to WarBOT")]
public class SetModule : WarBOTModule
{
    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("website", "Sets the website for this guild.")]
    public async Task Website([Summary("website", "Message to be displayed when /website is executed")] string Website)
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.Website = Website;
            await logic.SaveChangesAsync();
            await RespondAsync("The website value has been set to: " + Website);
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("role", "Updates the specified role in the configuration")]
    public async Task SetRole(
        [Summary("RoleLevel", "Which Role to update")] ConfigurableRoleLevel Role,
        [Summary("Role", "Guild role to use for this role.")] IRole GuildRole = null,
        [Summary("Clear", "If specified, will clear the specified role from the configuration.")] bool Clear = false
        )
    {
        await UseGuildLogicAsync(async logic =>
        {
            var ActualRoleLevel = Role.ToRoleLevel();
            var ExistingRole = logic.Roles.GetRole(ActualRoleLevel);

            if (Clear == true)
            {
                if (ExistingRole is null)
                {
                    await RespondAsync($"Role level {ActualRoleLevel} was not configured.");
                    return;
                }
                else
                {
                    logic.Roles.GetRoleLogic(ActualRoleLevel)?.ClearSettings();
                    await logic.SaveChangesAsync();
                    await RespondAsync($"Role level {ActualRoleLevel} has been removed.");
                }
                return;
            }


            var roleMap = logic.Roles.GetRoleMap();
            var duplicateRole = roleMap.Where(o => o.Key != ActualRoleLevel && o.Value is not null && o.Value.RoleID == GuildRole.Id);


            //Check for duplicate RoleLevels with the same configured role.
            if (duplicateRole.Any())
                await RespondAsync($"{GuildRole.Mention} is already being used for role level {duplicateRole.First().Key}.");
            //Make sure we are actually changing something.
            else if (ExistingRole is not null && ExistingRole.Id == GuildRole.Id)
                await RespondAsync($"Role {ActualRoleLevel} is already configured for {GuildRole.Mention}");
            else
            {
                logic.Roles.SetRole(ActualRoleLevel, GuildRole);
                await logic.SaveChangesAsync();
                await RespondAsync($"Updated role {ActualRoleLevel} to {GuildRole.Mention}");
            }
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("channel", "Sets the specified channel in the configuration")]
    public async Task SetChannel([Summary("Event", "Which Event to Manage")] Models.ChannelType Target, [Summary("Channel", "The target channel")] ITextChannel Channel = null)
    {
        await UseGuildLogicAsync(async logic =>
        {
            GuildChannelEventLogic? x = Target switch
            {
                Models.ChannelType.Portal => logic.HustleSettings.Event_Portal,
                Models.ChannelType.User_Greetings => logic.Event_UserJoin,
                Models.ChannelType.User_Leave => logic.Event_UserLeft,
                Models.ChannelType.WarBOT_Updates => logic.Event_Updates,
                _ => null,
            };

            async Task SaveandSendConfirmation()
            {
                await logic.SaveChangesAsync();

                if (Channel is null)
                    await RespondAsync($"Channel has been unset for {Target}");
                else
                    await RespondAsync($"Channel {Channel.Mention} will be used for {Target}");
            }


            if (x is not null)
            {
                x.Channel.SetChannel(Channel);
                await SaveandSendConfirmation();
                return;
            }

            ChannelLogic? y = Target switch
            {
                Models.ChannelType.Error_Messages => logic.Channel_Admins,
                _ => null
            };

            if (y is not null)
            {
                y.SetChannel(Channel);
                await SaveandSendConfirmation();
                return;
            }

            await RespondAsync($"Could not find event type.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("portal", "This will set the message displayed when the portal is opened, and enable the notification.")]
    public async Task SetPortalOpen([Summary("Message", "The message to display when the portal is opened.")] string Message)
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.HustleSettings.Event_Portal.Message = Message;
            await logic.SaveChangesAsync();
            await RespondAsync("The portal opening message has been set.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("loot", "Sets the loot for this guild.")]
    public async Task Loot([Summary("loot", "Message to be displayed when /loot is executed")] string Loot)
    {
        await UseGuildLogicAsync(async logic =>
        {
            logic.HustleSettings.LootMessage = Loot;
            await logic.SaveChangesAsync();
            await RespondAsync("The loot value has been set.");
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("greeting", "Enables message to a specific channel when users leave a discord guild.")]
    public async Task Greeting(
        [Summary("channel", "New user notifications will be sent here.")] ITextChannel Channel = null,
        [Summary("message", "Greeting Message. {User} will be replaced with joining user.")] string Message = null)
    {
        await UseGuildLogicAsync(async logic =>
        {

            //Update the channel in config, if it was specified.
            if (Channel != null)
                logic.Event_UserJoin.Channel.SetChannel(Channel);
            logic.Event_UserJoin.Enabled = true;

            var ch = await logic.Event_UserJoin.Channel.GetChannelAsync();

            //Both Null
            if (ch is null)
            {
                await RespondAsync("Please provide a valid channel.");
            }
            else if (!string.IsNullOrWhiteSpace(Message))
            {
                logic.Event_UserJoin.Message = Message;
                await RespondAsync($"I will greet new users in {ch.Mention} with this message: " + logic.Event_UserJoin.Message);
            }

            await logic.SaveChangesAsync();
        });
    }

    [RoleLevel(RoleLevel.Leader)]
    [SlashCommand("farewell", "Enables notification when a user leaves this guild.")]
    public async Task Farewell(
        [Summary("channel", "Notifications will be sent to this channel")] ITextChannel Channel = null,
        [Summary("message", "Farewell Message. {User} will be replaced with leaving user.")] string Message = null)
    {
        await UseGuildLogicAsync(async logic =>
        {

            //Update the channel in config, if it was specified.
            if (Channel != null)
                logic.Event_UserLeft.Channel.SetChannel(Channel);
            logic.Event_UserLeft.Enabled = true;

            var ch = await logic.Event_UserLeft.Channel.GetChannelAsync();

            //Both Null
            if (ch is null)
            {
                await RespondAsync("Please provide a valid channel.");
            }
            else if (!string.IsNullOrWhiteSpace(Message))
            {
                logic.Event_UserJoin.Message = Message;
                await RespondAsync($"I will send a farewall in {ch.Mention} with this message: " + logic.Event_UserLeft.Message);
            }

            await logic.SaveChangesAsync();
        });
    }


    [RoleLevel(RoleLevel.Officer)]
    [SlashCommand("nickname", "Change a user's nickname.")]
    [RequireBotPermission(GuildPermission.ManageNicknames)]
    public async Task Nickname(SocketGuildUser user, string Nickname)
    {
        var Me = Context.Guild.CurrentUser;
        if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
        {
            await RespondAsync("The provided nickname was not valid.");
            return;
        }

        //If WarBot was tagged, lets call the method to update his nickname.
        if (user.Id == Me.Id)
        {
            await Me.ModifyAsync(o => o.Nickname = Nickname);
            await RespondAsync("I have updated my nickname");
            return;
        }
        //Do a permissions check.
        else if (user.Hierarchy > Me.Hierarchy)
        {
            await RespondAsync($"The target user is a member of a higher role then I am. I cannot modify or manage that user.");
        }
        else
        {
            await user.ModifyAsync(o =>
            {
                o.Nickname = Nickname;
            });

            await RespondAsync($"I have changed {user.Mention}'s nickname.");
        }
    }
}

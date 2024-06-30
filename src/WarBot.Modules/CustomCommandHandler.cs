using Hangfire;
using WarBot.Data;

namespace WarBot.Modules;
public class CustomCommandHandler : WarBOTModule
{
    private readonly IServiceProvider sp;

    public CustomCommandHandler(IServiceProvider sp)
    {
        this.sp = sp;
    }

    public async Task<bool> HandleCommand(IInteractionContext args)
    {
        if (args.Interaction is null)
            return false;

        if (args.Interaction is SocketSlashCommand slash)
        {
            //Defer while we think about it.
            await args.Interaction.DeferAsync(true);
            using var scope = sp.CreateAsyncScope();
            using var db = scope.ServiceProvider.GetService<WarDB>();
            var log = scope.ServiceProvider.GetService<ILogger<CustomCommandHandler>>();

            log.LogDebug($"Received custom command for {slash.CommandName}.");
            try
            {
                using var logic = await GuildLogic.GetOrCreateAsync(args.Client, db, args.Guild);
                var matchingcommand = await logic.GetSlashCommandAsync(slash.CommandName);
                if (matchingcommand is null) return false;
                await ProcessCustomCommand(args, slash, matchingcommand);
                return true;
            }
            catch (Discord.Net.HttpException hex) when (hex.DiscordCode == DiscordErrorCode.InsufficientPermissions)
            {
                await args.Interaction.ModifyOriginalResponseAsync(o =>
                {
                    o.Content = "I have insufficient permissions to perform this action.";
                    o.Flags = MessageFlags.Ephemeral;
                });
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Fatal error while processing custom command");
                await args.Interaction.RespondAsync("Something broke.");
            }
        }

        return false;
    }

    private async Task ProcessCustomCommand(IInteractionContext ctx, SocketSlashCommand cmd, CustomCommandLogic Logic)
    {
        bool ErrorsDetected = false;
        foreach (var action in Logic.Actions)
        {
            try
            {
                switch (action.Type)
                {
                    case Data.Models.CustomCommandActionType.REPLY_WITH_MESSAGE:
                        await ctx.Interaction.ModifyOriginalResponseAsync(o => o.Content = action.Message);
                        break;
                    case Data.Models.CustomCommandActionType.BROADCAST_MESSAGE_TARGET_CHANNEL:
                        var ch = await action.TargetChannel.GetChannelAsync();
                        await ch.SendMessageAsync(action.Message);
                        break;
                    case Data.Models.CustomCommandActionType.ADD_ROLE_CALLING_USER:
                        {
                            var role = action.TargetRole.GetRole();
                            var suser = cmd.User as SocketGuildUser;
                            await suser.AddRoleAsync(role);
                            break;
                        }
                    case Data.Models.CustomCommandActionType.REMOVE_ROLE_CALLING_USER:
                        {
                            var role = action.TargetRole.GetRole();
                            var suser = cmd.User as SocketGuildUser;
                            await suser.RemoveRoleAsync(role);
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Discord.Net.HttpException hex) when (hex.DiscordCode == DiscordErrorCode.InsufficientPermissions)
            {
                string Message = $@"Custom Command '{cmd.CommandName}' has action '{action.Description}', for which I have insufficient permissions to execute. 
Please either correct my permissions, or remove this action.";
                BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(ctx.Guild.Id, Message));
                ErrorsDetected = true;
            }
            catch (Discord.Net.HttpException hex) when (hex.DiscordCode == DiscordErrorCode.MissingPermissions)
            {
                string Message = $@"Custom Command '{cmd.CommandName}' has action '{action.Description}', for which I missing permissions to execute. 
Please either correct my permissions, or remove this action.";
                BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(ctx.Guild.Id, Message));
                ErrorsDetected = true;
            }
        }

        if (!Logic.Actions.Any(o => o.Type == Data.Models.CustomCommandActionType.REPLY_WITH_MESSAGE))
        {
            if (!ErrorsDetected)
                await ctx.Interaction.ModifyOriginalResponseAsync(o => { o.Content = "Done."; o.Flags = MessageFlags.Ephemeral; });
            else
                await ctx.Interaction.ModifyOriginalResponseAsync(o => { o.Content = "I could not fully execute this command. I have notified your guild's administrator."; o.Flags = MessageFlags.Ephemeral; });
        }
    }


}


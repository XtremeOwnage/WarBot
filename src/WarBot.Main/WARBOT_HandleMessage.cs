using Discord.Interactions;
using WarBot.Core;
using WarBot.Modules.Base;
using static WarBot.Modules.Base.RoleLevelAttribute;

namespace WarBot;
public partial class WARBOT
{
    private async Task HandleInteractionServiceError(IInteractionContext arg2, IResult arg3)
    {
        if (!arg3.IsSuccess)
        {
            switch (arg3.Error)
            {
                case InteractionCommandError.UnmetPrecondition when arg3 is AccessDeniedPreconditionResult access:
                    await arg2.Interaction.RespondAsync($"You do not have access to this command. You require the {access.RequiredRole} role.");
                    break;
                case InteractionCommandError.UnmetPrecondition:
                    await arg2.Interaction.RespondAsync($"Unmet Precondition: {arg3.ErrorReason}");
                    break;
                case InteractionCommandError.UnknownCommand:
                    {
                        var s = sp.GetService<WarBot.Modules.CustomCommandHandler>();
                        if (await s.HandleCommand(arg2))
                        {
                            MetricHelper.AddCustomExecution();
                            return;
                        }
                    }
                    await arg2.Interaction.RespondAsync("Unknown command");
                    break;
                case InteractionCommandError.BadArgs:
                    await arg2.Interaction.RespondAsync("Invalid number or arguments");
                    break;
                case InteractionCommandError.Exception:
                    await arg2.Interaction.RespondAsync($"Command exception:{arg3.ErrorReason}");
                    break;
                case InteractionCommandError.Unsuccessful:
                    await arg2.Interaction.RespondAsync("Command could not be executed");
                    break;
                case InteractionCommandError.ConvertFailed:
                    await arg2.Interaction.RespondAsync("I failed to interpret the arguments");
                    break;
                default:
                    break;
            }

            MetricHelper.AddInteractionFailure();
            Console.WriteLine("Crap?");
        }
    }

    private async Task SlashCommandExecuted(SocketInteraction arg)
    {
        MetricHelper.AddSlashCommandExecution();
        var ctx = new WarBOTInteractionContext(client, arg, sp);
        await Interactionservice.ExecuteCommandAsync(ctx, sp);
    }

    private async Task MessageContextCommandExecuted(SocketInteraction arg)
    {
        MetricHelper.AddMessageContextExecution();
        var ctx = new WarBOTInteractionContext(client, arg, sp);
        await Interactionservice.ExecuteCommandAsync(ctx, sp);
    }

    private async Task UserContextCommandExecuted(SocketInteraction arg)
    {
        MetricHelper.AddUserContextExecution();
        var ctx = new WarBOTInteractionContext(client, arg, sp);
        await Interactionservice.ExecuteCommandAsync(ctx, sp);
    }




    //private Task Client_MessageReceived(SocketMessage socketMessage)
    //{
    //    Interlocked.Increment(ref this.MessagesProcessed);

    //    var message = socketMessage as SocketUserMessage;

    //    //If this was a system message, ignore it.
    //    if (message == null)
    //        return Task.CompletedTask;
    //    //If the message is from a bot, ignore it.
    //    else if (message.Author.IsBot)
    //        return Task.CompletedTask;
    //    //If the message is from me, ignore it.
    //    else if (message.Author.Id == client.CurrentUser.Id)
    //        return Task.CompletedTask;

    //    var t = Task.Run(() => processMessage(message));

    //    return Task.CompletedTask;
    //}
    //private async Task processMessage(SocketUserMessage message)
    //{
    //    try
    //    {
    //        //Start actual processing logic.              
    //        var UserChannelHash = SocketDialogContextBase.GetHashCode(message.Channel, message.Author);
    //        //Check if there is an open dialog.
    //        //ToDo - If the hash logic is perfectly sound, we can remove the second check to improve performance.
    //        //This case, is outside of the channel type comparison, because a dialog can occur in many multiple channel types.
    //        if (this.Dialogs.ContainsKey(UserChannelHash) && this.Dialogs[UserChannelHash].InContext(message.Channel.Id, message.Author.Id))
    //        {
    //            await this.Dialogs[UserChannelHash].ProcessMessage(message);
    //        }
    //        //Socket GUILD TEXT Channel.
    //        else if (message.Channel is SocketTextChannel tch)
    //        {
    //            try
    //            {
    //                var cfg = await GuildLogic.GetOrCreateAsync(this.sp, tch.Guild);

    //                #region Parse out command from prefix.
    //                int argPos = 0;
    //                bool HasPrefix = message.HasStringPrefix(cfg?.BotPrefix ?? "bot,", ref argPos, StringComparison.OrdinalIgnoreCase)
    //                    || message.HasMentionPrefix(client.CurrentUser, ref argPos);
    //                #endregion


    //                //If the config is null, and we are not setting the environment, return.
    //                if (cfg == null)
    //                    return;
    //                //If the message was not to me, Ignore it.
    //                else if (!HasPrefix)
    //                    return;

    //                //Strip out the prefix.
    //                string Msg = message.Content
    //                    .Substring(argPos, message.Content.Length - argPos)
    //                    .Trim()
    //                    .RemovePrecedingChar(',');


    //                //Load dynamic command context.
    //                var context = new GuildCommandContext(client, message, sp);

    //                var result = await Interactionservice.ExecuteAsync(context, Msg, sp, MultiMatchHandling.Best);

    //                //Return an error to the user, if we can send to this channel.
    //                if (!result.IsSuccess && PermissionHelper.TestBotPermission(context, Discord.ChannelPermission.SendMessages))
    //                {
    //                    switch (result.Error.Value)
    //                    {
    //                        case CommandError.UnknownCommand:
    //                            break;
    //                        case CommandError.ParseFailed:
    //                            break;
    //                        case CommandError.BadArgCount:
    //                            break;
    //                        case CommandError.ObjectNotFound:
    //                            break;
    //                        case CommandError.MultipleMatches:
    //                            break;
    //                        case CommandError.UnmetPrecondition when result is AccessDeniedPreconditionResult access:
    //                            await tch.SendMessageAsync($"You do not have access to this command. You require the {access.RequiredRole.ToString()} role.");
    //                            break;
    //                        case CommandError.UnmetPrecondition when result is PreconditionResult res:
    //                            await tch.SendMessageAsync(res.ErrorReason);
    //                            break;
    //                        case CommandError.Exception:
    //                            await tch.SendMessageAsync("An error has occured. The details will be reported for remediation.");
    //                            break;
    //                        case CommandError.Unsuccessful:
    //                            break;
    //                    }
    //                }

    //                //await this.log(message, tch.Guild, result);
    //            }
    //            catch (Exception ex)
    //            {
    //                log.LogError(ex.Message);
    //                return;
    //            }
    //        }
    //        else if (message.Channel is SocketDMChannel dm)
    //        {
    //            var context = new CommandContext(client, message);

    //            var result = await Interactionservice.ExecuteAsync(context, message.Content, sp, MultiMatchHandling.Best);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        log.LogError(ex.Message);
    //    }
    //}


}

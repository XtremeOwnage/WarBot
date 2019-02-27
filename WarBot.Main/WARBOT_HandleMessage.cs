using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.Dialogs;
using WarBot.Core.ModuleType;
using static WarBot.Attributes.RoleLevelAttribute;

namespace WarBot
{
    public partial class WARBOT
    {
        private Task Client_MessageReceived(SocketMessage socketMessage)
        {
             Interlocked.Increment(ref this.MessagesProcessed);

            var message = socketMessage as SocketUserMessage;

            //If this was a system message, ignore it.
            if (message == null)
                return Task.CompletedTask;
            //If the message is from a bot, ignore it.
            else if (message.Author.IsBot)
                return Task.CompletedTask;
            //If the message is from me, ignore it.
            else if (message.Author.Id == Client.CurrentUser.Id)
                return Task.CompletedTask;

            var t = Task.Run(() => processMessage(message));

            return Task.CompletedTask;
        }
        private async Task processMessage(SocketUserMessage message)
        {
            try
            {
                //Start actual processing logic.              
                var UserChannelHash = SocketDialogContextBase.GetHashCode(message.Channel, message.Author);
                //Check if there is an open dialog.
                //ToDo - If the hash logic is perfectly sound, we can remove the second check to improve performance.
                //This case, is outside of the channel type comparison, because a dialog can occur in many multiple channel types.
                if (this.Dialogs.ContainsKey(UserChannelHash) && this.Dialogs[UserChannelHash].InContext(message.Channel.Id, message.Author.Id))
                {
                    await this.Dialogs[UserChannelHash].ProcessMessage(message);
                }
                //Socket GUILD TEXT Channel.
                else if (message.Channel is SocketTextChannel tch)
                {
                    try
                    {
                        var cfg = await this.GuildRepo.GetConfig(tch.Guild);

                        #region Parse out command from prefix.
                        int argPos = 0;
                        bool HasPrefix = message.HasStringPrefix(cfg?.Prefix ?? "bot,", ref argPos, StringComparison.OrdinalIgnoreCase)
                            || message.HasMentionPrefix(Client.CurrentUser, ref argPos);
                        #endregion


                        //If the config is null, and we are not setting the environment, return.
                        if (cfg == null)
                            return;
                        //If the message was not to me, Ignore it.
                        else if (!HasPrefix)
                            return;

                        //Strip out the prefix.
                        string Msg = message.Content
                            .Substring(argPos, message.Content.Length - argPos)
                            .Trim()
                            .RemovePrecedingChar(',');


                        //Load dynamic command context.
                        var context = new GuildCommandContext(Client, message, cfg, this);

                        var result = await commands.ExecuteAsync(context, Msg, kernel, MultiMatchHandling.Best);

                        //Return an error to the user, if we can send to this channel.
                        if (!result.IsSuccess && PermissionHelper.TestBotPermission(tch, Discord.ChannelPermission.SendMessages))
                        {
                            switch (result.Error.Value)
                            {
                                case CommandError.UnknownCommand:
                                    break;
                                case CommandError.ParseFailed:
                                    break;
                                case CommandError.BadArgCount:
                                    break;
                                case CommandError.ObjectNotFound:
                                    break;
                                case CommandError.MultipleMatches:
                                    break;
                                case CommandError.UnmetPrecondition when result is AccessDeniedPreconditionResult access:
                                    await tch.SendMessageAsync($"You do not have access to this command. You require the {access.RequiredRole.ToString()} role.");
                                    break;
                                case CommandError.UnmetPrecondition when result is PreconditionResult res:
                                    await tch.SendMessageAsync(res.ErrorReason);
                                    break;
                                case CommandError.Exception:
                                    await tch.SendMessageAsync("An error has occured. The details will be reported for remediation.");
                                    break;
                                case CommandError.Unsuccessful:
                                    break;
                            }
                        }

                        await Log.ChatMessage(message, tch.Guild, result);
                    }
                    catch(Exception ex)
                    {
                        await Log.Error(tch.Guild, ex);
                        return;
                    }
                }
                else if (message.Channel is SocketDMChannel dm)
                {
                    var context = new Core.ModuleType.CommandContext(Client, message, this);

                    var result = await commands.ExecuteAsync(context, message.Content, kernel, MultiMatchHandling.Best);
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }


    }
}

using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using WarBot.Core.Dialogs;
using WarBot.Core.ModuleType;

namespace WarBot
{
    public partial class WARBOT
    {
        private async Task Client_MessageReceived(SocketMessage socketMessage)
        {
            Interlocked.Increment(ref this.MessagesProcessed);
            try
            {
                var message = socketMessage as SocketUserMessage;

                //If this was a system message, ignore it.
                if (message == null)
                    return;

                //If the message is from a bot, ignore it.
                if (message.Author.IsBot)
                    return;

                //If the message is from me, ignore it.
                if (message.Author.Id == Client.CurrentUser.Id)
                    return;

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
                    var cfg = await this.GuildRepo.GetConfig(tch.Guild);

                    #region Parse out command from prefix.
                    int argPos = 0;
                    bool HasStringPrefix = message.HasStringPrefix(cfg?.Prefix ?? "bot,", ref argPos, StringComparison.OrdinalIgnoreCase);
                    bool HasBotPrefix = message.HasMentionPrefix(Client.CurrentUser, ref argPos);

                    //Substring containing only the desired commands.
                    string Msg = message.Content.Substring(argPos, message.Content.Length - argPos).Trim();
                    #endregion


                    //If the config is null, and we are not setting the environment, return.
                    if (cfg == null)
                        return;
                    //If the message was not to me, Ignore it.
                    else if (!(HasStringPrefix || HasBotPrefix))
                        return;

                    //Load dynamic command context.
                    var context = new GuildCommandContext(Client, message, cfg, this);

                    var result = await commands.ExecuteAsync(context, Msg, kernel, MultiMatchHandling.Best);

                    await Log.ChatMessage(message, tch.Guild, result);
                }
                else if (message.Channel is SocketDMChannel dm)
                {
                    var context = new Core.ModuleType.CommandContext(Client, message, this);

                    var result = await commands.ExecuteAsync(context, socketMessage.Content, kernel, MultiMatchHandling.Best);
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }


    }
}

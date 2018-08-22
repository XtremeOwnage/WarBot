using DiscordBotsList.Api;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WarBot.Storage;
using Discord.Commands;
using System.Threading;

namespace WarBot
{
    public partial class WARBOT
    {
        private async Task Client_MessageReceived(SocketMessage socketMessage)
        {
            Interlocked.Increment(ref this.MessagesProcessed);
            try
            {
                //If the message is from a bot, ignore it.
                if (socketMessage.Author.IsBot)
                    return;

                //If the message is from me, ignore it.
                if (socketMessage.Author.Id == Client.CurrentUser.Id)
                    return;

                //If the message came from a logging channel, ignore it.
                if (Log.IsLoggingChannel(socketMessage.Channel.Id))
                    return;

                var message = socketMessage as SocketUserMessage;

                //If this was a system message, ignore it.
                if (message == null)
                    return;

                #region Parse out command from prefix.
                int argPos = 0;
                bool HasStringPrefix = message.HasStringPrefix("bot,", ref argPos, StringComparison.OrdinalIgnoreCase);
                bool HasBotPrefix = message.HasMentionPrefix(Client.CurrentUser, ref argPos);

                //Substring containing only the desired commands.
                string Msg = message.Content.Substring(argPos, message.Content.Length - argPos).Trim();
                #endregion

                //Start actual processing logic.
                if (message.Channel is SocketTextChannel tch)
                {
                    //If the message was not to me, Ignore it.
                    if (!(HasStringPrefix || HasBotPrefix))
                        return;

                    var cfg = await this.GuildRepo.GetConfig(tch.Guild);

                    //Compares the guilds environment with the current processes environment.
                    if (!ShouldHandleMessage(cfg))
                        return;

                    //Load dynamic command context.
                    var context = new SocketCommandContext(Client, message);

                    var result = await commands.ExecuteAsync(context, Msg, services, MultiMatchHandling.Best);

                    await Log.ChatMessage(message, tch.Guild, result);
                }
                else if (message.Channel is SocketDMChannel dm)
                {
                    //Load dynamic command context.
                    await dm.SendMessageAsync("Sorry, I don't yet support direct messages. My developer is working on this functionality though.");
                    await dm.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
        }


    }
}

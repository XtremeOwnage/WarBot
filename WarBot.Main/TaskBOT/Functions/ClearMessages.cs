using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarBot.TaskBOT
{
    public partial class TaskBOT
    {
        public async Task ClearMessages(SocketTextChannel Channel, bool DeletePinned = false)
        {
            try
            {
                DateTimeOffset discordBulkCutoffDate = DateTimeOffset.Now.AddDays(-10);

                //Bulk Delete messages
                while (true)
                {
                    IAsyncEnumerable<IReadOnlyCollection<IMessage>> asyncresults = Channel.GetMessagesAsync(500);
                    IEnumerable<IMessage> results = await asyncresults.FlattenAsync();

                    List<IMessage> ToBulkDelete = results
                    .Where(o => o.CreatedAt > discordBulkCutoffDate)
                    .ToList();

                    if (DeletePinned == false)
                        ToBulkDelete = ToBulkDelete.Where(o => o.IsPinned == false).ToList();


                    //If there are messages to bulk delete, do it.
                    if (ToBulkDelete.Count > 0)
                        await Channel.DeleteMessagesAsync(ToBulkDelete);
                    else
                        break;

                }
                RequestOptions options = RequestOptions.Default;
                options.Timeout = (int)TimeSpan.FromMinutes(5).TotalSeconds;
                options.RetryMode = RetryMode.RetryRatelimit;
                options.CancelToken = BOT.StopToken.Token;

                ITextChannel ch = Channel as ITextChannel;
                int FailCount = 0;
                while (true)
                {
                    try
                    {                       

                        List<IMessage> msgs = ch.GetMessagesAsync(1, CacheMode.AllowDownload, options).FlattenAsync().Result.ToList();

                        if (msgs.Count == 0)
                            break;

                        foreach (IMessage msg in msgs)
                        {
                            await msg.DeleteAsync(options);

                            await Task.Delay(50);
                        }
                    }
                    catch(System.TimeoutException)
                    {
                        FailCount++;

                        if (FailCount == 20)
                        {
                            await Channel.SendMessageAsync("Encountered too many errors. Please run the command again later.");
                            throw;
                        }

                        await Log.ConsoleOUT("TaskBot - Bulk delete sleeping for 1 minute. Caught timeout");

                        await Channel.SendMessageAsync("-Sleeping for 1 minute, due to Discord's API policies.-");
                        //Wait 10 seconds.
                        await Task.Delay((int)TimeSpan.FromMinutes(1).TotalMilliseconds);
                    }
                }
            }
            catch (Exception ex)
            {
                await Log.Error(Channel.Guild, ex, "TaskBOT_ClearMessages_Bulk");
                throw;
            }
        }
    }
}

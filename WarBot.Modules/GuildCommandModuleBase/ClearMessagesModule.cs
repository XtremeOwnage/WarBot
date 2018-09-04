using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
using System.Linq;
using System;

namespace WarBot.Modules.GuildCommandModules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class ClearMessagesModule : GuildCommandModuleBase
    {
        [Command("clear messages"), Alias("clear", "purge")]
        [CommandUsage("{prefix} {command} [NonPinned, Pinned]")]
        [RoleLevel(RoleLevel.Leader)]
        [Summary("Delete specified messages from a channel. Will exclude pinned messages unless specified.")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        ///action
        ///NonPinned = Only non-pinned messages
        ///Pinned = Only Pinned Messages
        ///ALL = Everything!
        public async Task ClearMessages(string action = "NonPinned")
        {
            bool SelectPinned = action.ToLowerInvariant().Contains("pinned");

            DateTimeOffset discordBulkCutoffDate = DateTimeOffset.Now.AddDays(-13);
            while (true)
            {
                var asyncresults = this.Context.Channel.GetMessagesAsync(500);
                var results = await asyncresults.FlattenAsync();


                var matchingResults = results
                    .Where(o => o.IsPinned == SelectPinned);

                var ToBulkDelete = matchingResults
                    .Where(o => o.CreatedAt > discordBulkCutoffDate);

                try
                {
                    //If there are messages to bulk delete, do it.
                    if (ToBulkDelete.Count() > 0)
                        await Context.GuildChannel.DeleteMessagesAsync(ToBulkDelete);
                    //Once everything has been bulk deleted, start deleting one by one.
                    else if (matchingResults.Count() > 0)
                        foreach (IMessage msg in matchingResults)
                            await msg.DeleteAsync();
                    //If nothing else to delete, stop deleting stuff.
                    else
                        break;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

        }
    }
}
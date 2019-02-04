using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

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
        [RequireBotPermission(ChannelPermission.SendMessages)]
        ///action
        ///NonPinned = Only non-pinned messages
        ///Pinned = Only Pinned Messages
        ///ALL = Everything!
        public async Task ClearMessages(string action = "NonPinned")
        {
            bool SelectPinned = action.ToLowerInvariant().Equals("pinned");

            if (!Context.GuildChannel.TestBotPermission(ChannelPermission.ManageMessages))
            {
                await ReplyAsync("I require the MANAGE_MESSAGES permission to perform this action.");
                return;
            }

            await bot.TaskBot.ClearMessages(Context.GuildChannel, SelectPinned);
        }
    }
}
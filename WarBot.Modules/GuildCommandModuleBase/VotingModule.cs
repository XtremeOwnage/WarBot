using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;
using WarBot.Modules.Dialogs;

namespace WarBot.Modules.GuildCommandModules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class VotingModule : GuildCommandModuleBase
    {
        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} (TimeSpan) Your question here")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote(TimeSpan HowLong, [Remainder]string Question)
        {
            if (String.IsNullOrEmpty(Question))
                await bot.OpenDialog(new PollQuestionEntryDialog(this.Context, HowLong));
            else
                await bot.OpenDialog(new PollQuestionEntryDialog(this.Context, Question, HowLong));
        }

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} (timespan)")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoTime(TimeSpan Howlong) => await StartVote(Howlong, null);

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command} Your question here")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoTime([Remainder]string Question) => await StartVote(TimeSpan.FromMinutes(5), Question);

        [Command("start vote"), Alias("vote", "new vote", "poll", "new poll", "start poll")]
        [Summary("Starts a vote, with configurable options.")]
        [CommandUsage("{prefix} {command}")]
        [RoleLevel(RoleLevel.None, RoleMatchType.GREATER_THEN_OR_EQUAL)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task StartVote_NoParams() => await StartVote_NoTime(null);
    }

   
}
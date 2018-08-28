﻿using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class HelpModule : GuildCommandModuleBase
    {

        [Command("show help"), Alias("?", "help")]
        [Summary("Show commands you have access to. This is the command you are currently using.")]
        [CommandUsage("{prefix} help")]
        [RoleLevel(RoleLevel.None)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Help()
        {
            var Userrole = Context.GuildUser.GetRole(Context.cfg);
            var Commands = bot.CommandService.Commands;

            //Find commands, to which the user has access to.
            var matchedCommands = Commands
                .Where(o =>
                    (o.Preconditions.OfType<RoleLevelAttribute>().FirstOrDefault().IsNotNull(out var x) && x.hasPermission(Userrole) == true))
                .OrderBy(o => o.Name)
                .Select(o => new
                {
                    o.Name,
                    o.Summary,
                    o.Attributes.OfType<CommandUsageAttribute>().FirstOrDefault()?.Usage,
                    Aliases = o.Aliases.Skip(1)
                })
                .ToArray();


            int count = 0;
            int maxPerPage = 24;
            int page = 0;

            while (count < matchedCommands.Length)
            {
                var eb = new EmbedBuilder()
                    .WithTitle($"Commands ({page})");

                while (count - (page * maxPerPage) < maxPerPage && count < matchedCommands.Length)
                {
                    var i = matchedCommands[count];
                    var desc = new StringBuilder();

                    if (!String.IsNullOrEmpty(i.Summary))
                        desc.AppendLine("**Summary:** " + i.Summary);
                    if (i.Aliases.Count() > 0)
                        foreach (var a in i.Aliases)
                            desc.AppendLine($"**Alias:** {a}");
                    if (!string.IsNullOrEmpty(i.Usage))
                        desc.AppendLine("**Usage:** " + i.Usage);

                    eb.AddField_ex(i.Name, desc.ToString(), false);
                    count++;
                }

                await ReplyAsync("", embed: eb);
                page++;
            }


            //A list of commands has been compiled. Lets start sending embeds.



        }
    }
}
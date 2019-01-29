﻿using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.CommandModuleBase
{

    public class HelpModule : WarBot.Core.ModuleType.CommandModuleBase
    {
        [Command("about")]
        [Summary("Show some information about me.")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task About()
        {
            StringBuilder sb = new StringBuilder();

            sb
                .AppendLine("I am a discord bot designed around the mobile game Hustle Castle")
                .AppendLine()
                .AppendLine("My primary focus is to let you know when recurring in-game events are starting or ending.")
                .AppendLine("I also provide functionality around role management, voting, and a few other useful discord-related actions.")
                .AppendLine("If you would like to see me in your server, or would like more information, please visit this page:")
                .AppendLine("https://github.com/XtremeOwnage/WarBot/blob/master/README.md");

            await Context.Channel.SendMessageAsync(sb.ToString());
        }

        [Command("show help"), Alias("?", "help")]
        [Summary("Show commands you have access to. This is the command you are currently using.")]
        [CommandUsage("{prefix} help")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Help()
        {
            var Commands = bot.CommandService.Commands;

            await ReplyAsync("My developer has not yet finished the show help command for non-guild contexes.");

            ////Find commands, to which the user has access to.
            //var matchedCommands = Commands
            //    .Where(o => o.Module.)
            //    .Where(o =>

            //    .OrderBy(o => o.Name)
            //    .Select(o => new
            //    {
            //        o.Name,
            //        o.Summary,
            //        o.Attributes.OfType<CommandUsageAttribute>().FirstOrDefault()?.Usage,
            //        Aliases = o.Aliases.Skip(1)
            //    })
            //    .ToArray();


            //int count = 0;
            //int maxPerPage = 24;
            //int page = 0;

            //while (count < matchedCommands.Length)
            //{
            //    var eb = new EmbedBuilder()
            //        .WithTitle($"Commands ({page})");

            //    while (count - (page * maxPerPage) < maxPerPage && count < matchedCommands.Length)
            //    {
            //        var i = matchedCommands[count];
            //        var desc = new StringBuilder();

            //        if (!String.IsNullOrEmpty(i.Summary))
            //            desc.AppendLine("**Summary:** " + i.Summary);
            //        if (i.Aliases.Count() > 0)
            //            foreach (var a in i.Aliases)
            //                desc.AppendLine($"**Alias:** {a}");
            //        if (!string.IsNullOrEmpty(i.Usage))
            //            desc.AppendLine("**Usage:** " + i.Usage);

            //        eb.AddField(i.Name, desc.ToString());
            //        count++;
            //    }

            //    await ReplyAsync(embed: eb.Build());
            //    page++;
            //}


            //A list of commands has been compiled. Lets start sending embeds.



        }
    }
}
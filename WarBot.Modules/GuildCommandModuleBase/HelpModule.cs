using Discord;
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

    public class HelpModule : GuildCommandModuleBase
    {

        [Command("show help"), Alias("?", "help")]
        [Summary("Show commands you have access to. This is the command you are currently using.")]
        [CommandUsage("{prefix} {command}")]
        [RoleLevel(RoleLevel.None)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task Help(string CH = "")
        {
            try
            {
                RoleLevel Userrole = Context.GuildUser.GetRole(Context.cfg);
                System.Collections.Generic.IEnumerable<CommandInfo> Commands = bot.CommandService.Commands;

                IMessageChannel ch = Context.GuildChannel as IMessageChannel;
                if (!CH.Equals("CH", StringComparison.OrdinalIgnoreCase))
                    ch = await Context.GuildUser.GetOrCreateDMChannelAsync() as IMessageChannel;

                //Find commands, to which the user has access to.
                var matchedCommands = Commands
                .Where(o =>
                    (o.Preconditions.OfType<RoleLevelAttribute>().FirstOrDefault().IsNotNull(out RoleLevelAttribute x) && x.hasPermission(Userrole) == true)
                    || (o.Preconditions.OfType<RoleLevelAttribute>().FirstOrDefault() == null)
                    )
                .OrderBy(o => o.Name)
                .Select(o => new
                {
                    o.Name,
                    o.Summary,
                    Usage = o.Attributes.OfType<CommandUsageAttribute>().FirstOrDefault()?.GetUsage(o, Context.cfg),
                    Aliases = o.Aliases.Skip(1)
                })
                .ToArray();


                int count = 0;
                int maxPerPage = 24;
                int page = 0;

                while (count < matchedCommands.Length)
                {
                    StringBuilder sb = new StringBuilder();

                    while (sb.Length < 1500 && count < matchedCommands.Length)
                    {
                        var i = matchedCommands[count];

                        sb.AppendLine($"**{i.Name}**");
                        if (!string.IsNullOrEmpty(i.Summary))
                            sb.AppendLine("\t**Summary:**" + i.Summary);
                        if (i.Aliases.Count() > 0)
                            foreach (string a in i.Aliases)
                                sb.AppendLine($"\t**Alias:** {a}");
                        if (!string.IsNullOrEmpty(i.Usage))
                            sb.AppendLine("\t**Usage:** " + i.Usage);

                        count++;
                    }
                    await ch.SendMessageAsync(sb.ToString());

                    page++;
                }


                //A list of commands has been compiled. Lets start sending embeds.

            }
            catch (Exception ex)
            {
                await bot.Log.Error(Context.Guild, ex, nameof(Help));
                throw;
            }

        }
    }
}
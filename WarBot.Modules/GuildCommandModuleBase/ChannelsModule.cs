using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    /// <summary>
    /// This module manages a user's roles.
    /// </summary>
    public class ChannelsModule : GuildCommandModuleBase
    {
        [Command("set channel")]
        [Summary("Updates my channel mappings"), RoleLevel(RoleLevel.Leader), RequireBotPermission(GuildPermission.SendMessages)]
        [CommandUsage("{prefix}, set channel (Channel) #Channel")]
        public async Task UpdateChannel(string Role = "INVALID CHANNEL", SocketTextChannel Channel = null)
        {
            var Me = Context.Guild.CurrentUser;

            StringBuilder sb = new StringBuilder();

            if (System.Enum.TryParse(Role, true, out WarBotChannelType chType))
            {
                if (Channel == null)
                {
                    //Get the using attribute
                    var usage = this.GetType().GetCustomAttributes(typeof(CommandUsageAttribute), false)
                        .OfType<CommandUsageAttribute>()
                        .FirstOrDefault()
                        .Usage;
                    sb.AppendLine("Please remember to tag a channel for this command. Proper Syntex:")
                        .AppendLine(usage);
                }
                else
                {
                    #region Check to ensure WarBOT can manage all of the configured roles
                    var MyHighestRole = Me.Roles.Max(o => o.Position);

                    if (Channel.Position >= MyHighestRole)
                        sb.AppendLine($"Note: I will not be able to add or remove users from {Channel.Mention}, because this role is equal to, or above my current role. Please adjust my role position higher to compensate for this.");
                    #endregion

                    cfg.SetGuildChannel(chType, Channel);

                    await cfg.SaveConfig();

                    sb.AppendLine($"Channel {Channel.Mention} has been assigned for purpose {chType.ToString()}");
                }
            }
            else
            {

                //Was unable to parse a channel type from the input text.
                sb.AppendLine("I was unable to parse the desired channel type from your input. The accepted values are:");
                var validValues = Enum.GetValues(typeof(WarBotChannelType))
                    .OfType<WarBotChannelType>()
                    .Select(o => new
                    {
                        Name = o.ToString(),
                        Summary = o.GetEnumDescriptionAttribute()
                    });

                var table = TableHelper.FormatTable(validValues);


                sb.AppendLine($"```\r\n{table}\r\n```");
            }

            //Send the formatted return message.
            await ReplyAsync(sb.ToString());
        }
    }
}
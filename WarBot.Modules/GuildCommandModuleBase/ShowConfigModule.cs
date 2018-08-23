using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using System.Linq;
using System;
using WarBot.Core.ModuleType;
namespace WarBot.Modules.GuildCommandModules
{
    [RequireContext(ContextType.Guild)]
    public class ShowConfigModule : GuildCommandModuleBase
    {
       // ~say hello -> hello
        [Command("show config"), Alias("config show")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Display command stats related to me.")]
        [RequireBotPermission(Discord.ChannelPermission.SendMessages)]
        public async Task ShowConfig([Remainder]string Config = "ALL")
        {
            #region Determine which config parts to show
            const string c_ROLE = "role";
            const string c_CHANNEL = "channel";
            const string c_NOTIFICATION = "notification";
            const string c_BASIC = "basic";

            //Split on both comma and spaces.
            var parts = Config.Split(',', ' ');

            //normalize the parts.
            for (int i = 0; i < parts.Length; i++)
            {
                var str = parts[i].Trim();
                if (string.Equals(str, "all", StringComparison.OrdinalIgnoreCase))
                {
                    parts = new string[] { c_NOTIFICATION, c_CHANNEL, c_ROLE, c_BASIC };
                    break;
                }
                else if (String.Equals(str, "roles", StringComparison.OrdinalIgnoreCase))
                    parts[i] = c_ROLE;
                else if (String.Equals(str, "channels", StringComparison.OrdinalIgnoreCase))
                    parts[i] = c_CHANNEL;
                else if (String.Equals(str, "notifications", StringComparison.OrdinalIgnoreCase))
                    parts[i] = c_NOTIFICATION;
            }

            #endregion
          
            #region Show Basic Configuration
            if (parts.Contains(c_BASIC))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Bot Configuration - Basic")
                    .AddField_ex("Website Text", cfg.Website)
                    .AddField_ex("Loot Text", cfg.Loot)
                    .AddField_ex("My Nickname", cfg.NickName);

                await ReplyAsync("", embed: embed);
            }
            #endregion
            #region Show Roles Config
            //Discord allows a maximum of 25 lines per embed.
            //At the time of writing this, there are far fewer roles configured. but, the support for batching is already implemented.

            if (parts.Contains(c_ROLE))
            {
                var Roles = cfg.GetRoleMap();
                int RolePage = 1;
                foreach (var roleGroup in Roles.Where(o => o.Value != null).OrderBy(o => o.Key).Batch(24))
                {
                    var roleEmbed = new EmbedBuilder()
                        .WithTitle($"Bot Configuration: Roles({RolePage})");
                    foreach (var role in roleGroup)
                        if (role.Value.IsMentionable)
                            roleEmbed.AddField_ex($"Role {role.Key}", role.Value.Mention);
                        else
                            roleEmbed.AddField_ex($"Role {role.Key}", role.Value.Name);
                    await ReplyAsync("", embed: roleEmbed);
                    //Aggregate the role page counter.
                    RolePage++;
                }
            }
            #endregion
            #region Show Channels Config
            if (parts.Contains(c_CHANNEL))
            {
                var Channels = cfg.GetChannelMap();
                int ChannelPage = 1;
                //Discord allows a maximum of 25 lines per embed.
                //At the time of writing this, there are far fewer channels configured. but, the support for batching is already implemented.
                foreach (var channelGroup in Channels.Where(o => o.Value != null).OrderBy(o => o.Key).Batch(24))
                {
                    var channelsEmbed = new EmbedBuilder()
                        .WithTitle($"Bot Configuration: Channels({ChannelPage})");
                    foreach (var channel in channelGroup)
                        channelsEmbed.AddField_ex($"Role {channel.Key}", channel.Value.Mention);
                    await ReplyAsync("", embed: channelsEmbed);
                    //Aggregate the role page counter.
                    ChannelPage++;
                }
            }
            #endregion
            #region Show Notifications Config
            if (parts.Contains(c_NOTIFICATION))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Bot Configuration - War Notifications")
                    .AddField_ex("War Prep Started Enabled", cfg.Notifications.WarPrepStarted)
                    .AddField_ex("War Prep Started Message", cfg.Notifications.WarPrepStartedMessage)
                    .AddField_ex("War Prep Ending Enabled", cfg.Notifications.WarPrepAlmostOver)
                    .AddField_ex("War Prep Ending Message", cfg.Notifications.WarPrepEndingMessage)
                    .AddField_ex("War Started Enabled", cfg.Notifications.WarStarted)
                    .AddField_ex("War Started Message", cfg.Notifications.WarStartedMessage)
                    .AddField_ex("War 1 Enabled (2am CST)", cfg.Notifications.War1Enabled)
                    .AddField_ex("War 2 Enabled (8am CST)", cfg.Notifications.War2Enabled)
                    .AddField_ex("War 3 Enabled (2pm CST)", cfg.Notifications.War3Enabled)
                    .AddField_ex("War 4 Enabled (8pm CST)", cfg.Notifications.War4Enabled);

                await ReplyAsync("", embed: embed);
            }
            #endregion
        }
    }
}
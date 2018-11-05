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
    public class ShowConfigModule : GuildCommandModuleBase
    {
        [RoleLevel(RoleLevel.Officer)]
        [Command("show notifications")]
        [Summary("Shortcut for 'bot, show config notifications'")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ShowNotifications() => await ShowConfig("notification");

        [RoleLevel(RoleLevel.Officer)]
        [Command("show channels")]
        [Summary("Shortcut for 'bot, show config channel'")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ShowChannels() => await ShowConfig("channel");

        [RoleLevel(RoleLevel.Officer)]
        [Command("show roles")]
        [Summary("Shortcut for 'bot, show config role'")]
        [CommandUsage("{prefix} {command}")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ShowRoles() => await ShowConfig("role");

        [RoleLevel(RoleLevel.Officer)]
        [Command("show config"), Alias("config show")]
        [Summary("Display the configuration for this guild.")]
        [CommandUsage("{prefix} {command} [role, channel, notification, basic, ALL]")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
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
                    .WithTitle("Bot Configuration - Basic");

                if (!string.IsNullOrWhiteSpace(cfg.Website))
                    embed.AddField("Website Text", cfg.Website);
                if (!string.IsNullOrWhiteSpace(cfg.Loot))
                    embed.AddField("Loot Text", cfg.Loot);
                if (!string.IsNullOrWhiteSpace(cfg.Prefix))
                    embed.AddField("Prefix", cfg.Prefix);

                await ReplyAsync(embed: embed.Build());
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
                            roleEmbed.AddField($"Role {role.Key}", role.Value.Mention);
                        else
                            roleEmbed.AddField($"Role {role.Key}", role.Value.Name);
                    await ReplyAsync(embed: roleEmbed.Build());
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
                        channelsEmbed.AddField($"Role {channel.Key}", channel.Value.Mention);
                    await ReplyAsync(embed: channelsEmbed.Build());
                    //Aggregate the role page counter.
                    ChannelPage++;
                }
            }
            #endregion
            #region Show Notifications Config
            if (parts.Contains(c_NOTIFICATION))
            {
                var embed = new EmbedBuilder()
                    .WithTitle("Bot Configuration - Notifications")
                    .AddField("War Prep Started Enabled", cfg[Setting_Key.WAR_PREP_STARTED].Enabled)
                    .AddField("War Prep Ending Enabled", cfg[Setting_Key.WAR_PREP_ENDING].Enabled)
                    .AddField("War Started Enabled", cfg[Setting_Key.WAR_STARTED].Enabled);

                if (cfg[Setting_Key.WAR_PREP_STARTED].HasValue)
                    embed.AddField("War Prep Started Message", cfg[Setting_Key.WAR_PREP_STARTED].Value);
                if (cfg[Setting_Key.WAR_PREP_ENDING].HasValue)
                    embed.AddField("War Prep Ending Message", cfg[Setting_Key.WAR_PREP_ENDING].Value);
                if (cfg[Setting_Key.WAR_STARTED].HasValue)
                    embed.AddField("War Started Message", cfg[Setting_Key.WAR_STARTED].Value);

                embed
                    .AddField("War 1 Enabled (2am CST)", cfg[Setting_Key.WAR_1].Enabled)
                    .AddField("War 2 Enabled (8am CST)", cfg[Setting_Key.WAR_2].Enabled)
                    .AddField("War 3 Enabled (2pm CST)", cfg[Setting_Key.WAR_3].Enabled)
                    .AddField("War 4 Enabled (8pm CST)", cfg[Setting_Key.WAR_4].Enabled)
                    .AddField("Portal Remindar Enabled", cfg[Setting_Key.PORTAL_STARTED].Enabled);

                if (cfg[Setting_Key.PORTAL_STARTED].HasValue)
                    embed.AddField("Portal Started Message", cfg[Setting_Key.PORTAL_STARTED].Value);

                embed.AddField("User Join Enabled", cfg[Setting_Key.USER_JOIN].Enabled);
                embed.AddField("User Left Enabled", cfg[Setting_Key.USER_LEFT].Enabled);

                if (cfg[Setting_Key.USER_JOIN].HasValue)
                    embed.AddField("User Join Message", cfg[Setting_Key.USER_JOIN].Value);

                await ReplyAsync(embed: embed.Build());
            }
            #endregion
        }
    }
}
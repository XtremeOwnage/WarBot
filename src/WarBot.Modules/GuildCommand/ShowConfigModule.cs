using WarBot.Core.Extensions;
using WarBot.Core.Helper;
using WarBot.DataAccess.Logic.Events;

namespace WarBot.Modules.GuildCommand;
[RoleLevel(RoleLevel.Officer)]
[RequireContext(ContextType.Guild)]
[Group("show", "Display various pieces of information.")]
public class ShowConfigModule : WarBOTModule
{
    [RoleLevel(RoleLevel.Officer)]
    [SlashCommand("channels", "Shows configured channels")]
    public async Task ShowChannels()
    {
        await ShowConfig(Models.ShowConfigSection.Channels);
    }

    [SlashCommand("roles", "Shows configured roles")]
    public async Task ShowRoles()
    {
        await ShowConfig(Models.ShowConfigSection.Roles);
    }

    [SlashCommand("config", "Display the configuration for this guild.")]
    public Task ShowConfig([Summary("section", "Which configuration section to show")] Models.ShowConfigSection Section = Models.ShowConfigSection.ALL)
    {
        const string NOTSET = "[NOT SET]";
        return UseGuildLogicAsync(async logic =>
        {
            List<Embed> embeds = new List<Embed>();
            #region Show Basic Configuration
            if (Models.ShowConfigSection.Basic.HasFlag(Section))
            {
                EmbedBuilder embed = new EmbedBuilder()
                    .WithTitle("Bot Configuration - Basic");

                string website = StringHelper.Truncate(logic.Website, 500);
                string loot = StringHelper.Truncate(logic.HustleSettings.LootMessage, 500);

                embed.AddField("Website Text", website, NOTSET);
                embed.AddField("Loot Text", loot, NOTSET);

                embeds.Add(embed.Build());
            }
            #endregion
            #region Show Roles Config
            //Discord allows a maximum of 25 lines per embed.
            //At the time of writing this, there are far fewer roles configured. but, the support for batching is already implemented.

            if (Section.HasFlag(Models.ShowConfigSection.Roles))
            {
                var Roles = logic.Roles.GetRoleMap()
                    .Where(o => o.Value?.RoleID.HasValue ?? false);

                int RolePage = 1;
                foreach (var roleGroup in Roles.OrderBy(o => o.Key).Batch(24))
                {
                    EmbedBuilder roleEmbed = new EmbedBuilder()
                        .WithTitle($"Bot Configuration: Roles({RolePage})");
                    foreach (var role in roleGroup)
                    {
                        var r = role.Value.GetRole();
                        if (r is null)
                            continue;

                        if (r.IsMentionable)
                            roleEmbed.AddField($"Role {role.Key}", r.Mention, NOTSET);
                        else
                            roleEmbed.AddField($"Role {role.Key}", r.Name, NOTSET);
                    }
                    embeds.Add(roleEmbed.Build());
                    //Aggregate the role page counter.
                    RolePage++;
                }
            }
            #endregion

            #region Show Events
            if (Section.HasFlag(Models.ShowConfigSection.Events))
            {
                EmbedBuilder AddEvent(EmbedBuilder e, GuildChannelEventLogic Event, string Name)
                    => e.AddField($"{Name} - Enabled", Event.Enabled ? "Enabled" : "Disabled");


                EmbedBuilder embed = new EmbedBuilder().WithTitle("Bot Configuration: Events");
                AddEvent(embed, logic.Event_UserJoin, "User Greeting");
                AddEvent(embed, logic.Event_UserLeft, "User Leave");
                AddEvent(embed, logic.Event_Updates, "WarBOT Updates");

                embeds.Add(embed.Build());
            }
            #endregion

            #region Show Channels Config
            if (Section.HasFlag(Models.ShowConfigSection.Channels))
            {
                async Task<EmbedBuilder> AddChannelEvent(EmbedBuilder e, GuildChannelEventLogic Event, string Name)
                    => e.AddField($"{Name} - Channel", (await Event.Channel.GetChannelAsync())?.Mention, "[Not Set]");

                async Task<EmbedBuilder> AddChannel(EmbedBuilder e, ChannelLogic Channel, string Name)
                    => e.AddField($"{Name} - Channel", (await Channel.GetChannelAsync())?.Mention, "[Not Set]");

                EmbedBuilder embed = new EmbedBuilder().WithTitle($"Bot Configuration: Channels");
                await AddChannelEvent(embed, logic.Event_UserJoin, "User Greeting");
                await AddChannelEvent(embed, logic.Event_UserLeft, "User Leave");
                await AddChannelEvent(embed, logic.Event_Updates, "WarBOT Updates");

                await AddChannel(embed, logic.Channel_Admins, "Admin Notifications");

                embeds.Add(embed.Build());
            }

            #endregion

            #region Show Hustle Config

            if (Section.HasFlag(Models.ShowConfigSection.Hustle_Portal))
            {
                async Task<EmbedBuilder> AddSimpleEvent(EmbedBuilder e, GuildChannelEventLogic Event, string Name)
                {
                    string getClearMessage()
                    {
                        if (Event.ClearMethod == EventClearType.DISABLED)
                            return "Disabled";
                        else if (Event.ClearMethod == EventClearType.ENTIRE_CHANNEL)
                            return $"Clear entire channel {Event.ClearDurationMins} mins after start of event.";
                        else if (Event.ClearMethod == EventClearType.INDIVIDUAL_MESSAGE_TIMER)
                            return $"Remove individual messages {Event.ClearDurationMins} mins after being sent.";
                        else
                            return "Unknown";
                    }

                    return e
                        .AddField($"{Name} - Enabled", Event.Enabled)
                        .AddField($"{Name} - Channel", (await Event.Channel.GetChannelAsync())?.Mention, NOTSET)
                        .AddField($"{Name} - Message", Event.Message, NOTSET)
                        .AddField($"{Name} - Clear Method", getClearMessage());
                }

                EmbedBuilder embed2 = new EmbedBuilder().WithTitle($"Bot Configuration: Hustle Castle Portal");
                await AddSimpleEvent(embed2, logic.HustleSettings.Event_Portal, "Portal");
                embeds.Add(embed2.Build());

            }

            async Task<EmbedBuilder> AddHustleEvent(EmbedBuilder e, HustleGuildChannelEventLogic Event, string Name)
            {
                //Add null-check
                if (Event is null) return e;
                string getClearMessage()
                {
                    if (Event.ClearMethod == EventClearType.DISABLED)
                        return "Disabled";
                    else if (Event.ClearMethod == EventClearType.ENTIRE_CHANNEL)
                        return $"Clear entire channel {Event.ClearDurationMins} mins after start of event.";
                    else if (Event.ClearMethod == EventClearType.INDIVIDUAL_MESSAGE_TIMER)
                        return $"Remove individual messages {Event.ClearDurationMins} mins after being sent.";
                    else
                        return "Unknown";
                }

                //Note, There is a 25 field limit to embeds.
                // This means, each event (4 total) is limited to 6 fields each. 

                //Note 2- These are all seperate statements, instead of a giant chained lambda, to improve debuggability.
                //Reason being- error on line 162 could potentially indicate fault with ANY of the below statements.
                e.AddField($"{Name} - Enabled", Event.Enabled);
                e.AddField($"{Name} - Channel", (await Event.Channel.GetChannelAsync())?.Mention, NOTSET);
                e.AddField($"{Name} - Prep Started Message", Event.Prep_Started_Message, NOTSET);
                e.AddField($"{Name} - Prep Ending Message", Event.Prep_Ending_Message, NOTSET);
                e.AddField($"{Name} - Event Started Message", Event.Event_Started_Message, NOTSET);
                //.AddField($"{Name} - Event Ending Message", Event.Event_Finished_Message ?? "[Not Set]")
                e.AddField($"{Name} - Clear Method", getClearMessage());

                return e;
            }

            if (Section.HasFlag(Models.ShowConfigSection.Hustle_War))
            {
                EmbedBuilder embed = new EmbedBuilder().WithTitle($"Bot Configuration: Hustle Castle Wars");
                await AddHustleEvent(embed, logic.HustleSettings.Event_War_1, "War 1");
                await AddHustleEvent(embed, logic.HustleSettings.Event_War_2, "War 2");
                await AddHustleEvent(embed, logic.HustleSettings.Event_War_3, "War 3");
                await AddHustleEvent(embed, logic.HustleSettings.Event_War_4, "War 4");
                embeds.Add(embed.Build());
            }

            if (Section.HasFlag(Models.ShowConfigSection.Hustle_Expedition))
            {
                EmbedBuilder embed = new EmbedBuilder().WithTitle($"Bot Configuration: Hustle Castle Expeditions");
                await AddHustleEvent(embed, logic.HustleSettings.Event_Expedition_1, "Expedition 1");
                await AddHustleEvent(embed, logic.HustleSettings.Event_Expedition_2, "Expedition 2");
                await AddHustleEvent(embed, logic.HustleSettings.Event_Expedition_3, "Expedition 3");
                await AddHustleEvent(embed, logic.HustleSettings.Event_Expedition_4, "Expedition 4");
                embeds.Add(embed.Build());
            }

            #endregion
            //#region Show Notifications Config
            //if (parts.Contains(c_NOTIFICATION))
            //{
            //    EmbedBuilder embed = new EmbedBuilder()
            //        .WithTitle("Bot Configuration - Notifications")
            //        .AddField("War Prep Started Enabled", cfg[Setting_Key.WAR_PREP_STARTED].Enabled)
            //        .AddField("War Prep Ending Enabled", cfg[Setting_Key.WAR_PREP_ENDING].Enabled)
            //        .AddField("War Started Enabled", cfg[Setting_Key.WAR_STARTED].Enabled)
            //        .AddField("Clear War Channel When War Starts", cfg[Setting_Key.CLEAR_WAR_CHANNEL_ON_WAR_START].Enabled);

            //    if (cfg[Setting_Key.WAR_PREP_STARTED].HasValue)
            //        embed.AddField("War Prep Started Message", cfg[Setting_Key.WAR_PREP_STARTED].Value);
            //    if (cfg[Setting_Key.WAR_PREP_ENDING].HasValue)
            //        embed.AddField("War Prep Ending Message", cfg[Setting_Key.WAR_PREP_ENDING].Value);
            //    if (cfg[Setting_Key.WAR_STARTED].HasValue)
            //        embed.AddField("War Started Message", cfg[Setting_Key.WAR_STARTED].Value);

            //    embed
            //        .AddField("War 1 Enabled (7am UTC/GMT)", cfg[Setting_Key.WAR_1].Enabled)
            //        .AddField("War 2 Enabled (1pm UTC/GMT)", cfg[Setting_Key.WAR_2].Enabled)
            //        .AddField("War 3 Enabled (7pm UTC/GMT)", cfg[Setting_Key.WAR_3].Enabled)
            //        .AddField("War 4 Enabled (1am UTC/GMT)", cfg[Setting_Key.WAR_4].Enabled)
            //        .AddField("Portal Remindar Enabled", cfg[Setting_Key.PORTAL_STARTED].Enabled);

            //    if (cfg[Setting_Key.PORTAL_STARTED].HasValue)
            //        embed.AddField("Portal Started Message", cfg[Setting_Key.PORTAL_STARTED].Value);

            //    embed.AddField("User Join Enabled", cfg[Setting_Key.USER_JOIN].Enabled);
            //    if (cfg[Setting_Key.USER_JOIN].HasValue)
            //        embed.AddField("User Join Message", StringHelper.Truncate(cfg[Setting_Key.USER_JOIN].Value, 200));
            //    embed.AddField("User Left Enabled", cfg[Setting_Key.USER_LEFT].Enabled);
            //    if (cfg[Setting_Key.USER_LEFT].HasValue)
            //        embed.AddField("User Leave Message", StringHelper.Truncate(cfg[Setting_Key.USER_LEFT].Value, 200));



            //    await RespondAsync(embed: embed.Build());
            //}
            //#endregion

            if (!embeds.Any())
                await RespondAsync("Sorry, there is no configuration to show matching the provided parameters", ephemeral: true);
            else
            {
                await RespondAsync(embed: embeds.First());
                foreach (var additionalEmbed in embeds.Skip(1))
                    await FollowupAsync(embed: additionalEmbed);
            }
        });

    }


}
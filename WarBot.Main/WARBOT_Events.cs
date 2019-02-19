﻿using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core;
using System.Linq;

namespace WarBot
{
    public partial class WARBOT
    {
        private async Task Bot_GuildDeleted(SocketGuild arg)
        {
            var cfg = await this.GuildRepo.GetConfig(arg);
            await Log.ConsoleOUT($"Removed Guild: {arg.Name}");

            try
            {
                var matchingDialogs = this.Dialogs
                    .Where(o => o.Value is WarBot.Core.Dialogs.SocketGuildDialogContextBase sgd && sgd.Config == cfg)
                    .ToDictionary(o => o.Key, o => (WarBot.Core.Dialogs.SocketGuildDialogContextBase)o.Value);

                foreach (var Dialog in matchingDialogs)
                {
                    //Remove the dialog from the stack.
                    this.Dialogs.TryRemove(Dialog.Key, out var _);
                }

            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }

            //Log a message to the logging server.
            try
            {
                //Build embed.
                EmbedBuilder eb = new EmbedBuilder()
                    .WithTitle("Guild Deleted")
                    .WithColor(Color.Red)
                    .AddField("Guild", arg.Name)
                    .AddField("Members", arg.Users.Count);

                await Log.sendToChannel(LogChannel.GuildActivity, eb.Build());
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }
        }

        private async Task Bot_GuildAdded(SocketGuild arg)
        {
            var cfg = await this.GuildRepo.GetConfig(arg);
            await Log.ConsoleOUT($"New Guild: {arg.Name}");


            //Log a message to the logging server.
            try
            {
                //Build embed.
                EmbedBuilder eb = new EmbedBuilder()
                    .WithTitle("Guild Created")
                    .WithColor(Color.Green)
                    .AddField("Guild", arg.Name)
                    .AddField("Members", arg.Users.Count);

                await Log.sendToChannel(LogChannel.GuildActivity, eb.Build());
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }

            //Send a welcome message to the guild.
            try
            {
                var CH = await ChannelHelper.FindChannel_For_Welcome_Message(arg);
                if (CH != null)
                {

                    //Publish a Welcome Message.

                    var eb = new Discord.EmbedBuilder()
                        .WithTitle("WarBOT")
                        .WithColor(Color.Green)
                        .WithDescription("Thanks for inviting me to your server. I will send you notifications related to Hustle Castle war events.")
                        .AddField("_ _", "_ _")
                        .AddField("For Help", $"Just type 'bot, help' or {arg.CurrentUser.Mention} help")
                        .AddField("For Support", "Either click this message or contact <@381654208073433091>.")
                        .AddField("**Setup**", "Have a server admin type: **bot, setup**")
                        .WithUrl("https://github.com/XtremeOwnage/WarBot")
                        .WithImageUrl("http://i1223.photobucket.com/albums/dd516/ericmck2000/download.jpg");

                    await CH.SendMessageAsync(embed: eb.Build());
                }
                else
                {
                    await Log.Error(arg, new Exception("Unable to send my welcome message. No permissions."));
                }
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }

        }

        private async Task Client_Log(LogMessage message)
        {
            await Log.ConsoleOUT(message.Message);
        }

        private Task Client_UserJoined(SocketGuildUser arg)
        {
            var t = Task.Run(async () =>
            {
                var cfg = await this.GuildRepo.GetConfig(arg.Guild);

                //Send welcome message
                try
                {
                    if (cfg[Setting_Key.USER_JOIN].Enabled && cfg.GetGuildChannel(WarBotChannelType.USER_JOIN).IsNotNull(out var ch))
                    {
                        string Message = $"Welcome {arg.Mention}!";

                        if (cfg[Setting_Key.USER_JOIN].HasValue)
                        {
                            Message = cfg[Setting_Key.USER_JOIN].Value.Replace("{user}", arg.Mention, StringComparison.OrdinalIgnoreCase);
                        }

                        await ch.SendMessageAsync(text: Message);
                    }

                    //Guild must have configured both a new user greeting channel, as well as a greeting message.

                }
                catch (Exception ex)
                {
                    await Log.Error(arg.Guild, ex);
                }
            });
            return Task.CompletedTask;
        }

        private Task Client_UserLeft(SocketGuildUser arg)
        {
            var t = Task.Run(async () =>
            {
                var cfg = await this.GuildRepo.GetConfig(arg.Guild);
                var ch = cfg.GetGuildChannel(WarBotChannelType.USER_LEFT);
                if (cfg[Setting_Key.USER_LEFT].Enabled && ch != null)
                {
                    string name = !string.IsNullOrWhiteSpace(arg.Nickname) ? arg.Nickname : arg.Username;
                    string Message = $"{name} has left the guild.";

                    if (cfg[Setting_Key.USER_LEFT].HasValue)
                    {
                        Message = cfg[Setting_Key.USER_LEFT].Value.Replace("{user}", name, StringComparison.OrdinalIgnoreCase);
                    }

                    await ch.SendMessageAsync(text: Message);


                }
            });
            return Task.CompletedTask;
        }

        private async Task Client_RoleDeleted(SocketRole arg)
        {
            var cfg = await this.GuildRepo.GetConfig(arg.Guild);

            //We need to validate this role was not configured as any of this guild's current roles.
            var AffectedRoles = cfg.GetRoleMap().Where(o => o.Value.Id == arg.Id);

            //Nothing to do.
            if (AffectedRoles.Count() == 0)
                return;

            try
            {
                //Determine if there is an officers channel configured. If so, lets send a message.
                if (cfg.GetGuildChannel(WarBotChannelType.CH_Officers).IsNotNull(out var ch))
                {
                    var eb = new EmbedBuilder()
                        .WithTitle("Error: Role Deleted")
                        .WithDescription($"Role '{arg.Name}' was just deleted. This discord role was configured for these roles:");

                    foreach (var r in AffectedRoles)
                        eb.AddField("Role", r.Key.ToString());

                    eb.WithDescription("I will remove this role from my configuration. Please update the configuration if you wish to use it again.");

                    await ch.SendMessageAsync(embed: eb.Build());
                }
            }
            catch (Exception ex)
            {
                await Log.Error(arg.Guild, ex);
            }

            //Remove these roles from the warbot configuration.
            foreach (var role in AffectedRoles)
                cfg.SetGuildRole(role.Key, null);

            //Save changes.
            await cfg.SaveConfig();
        }

        private async Task Client_ChannelDestroyed(SocketChannel arg)
        {

            #region Close any open dialogs in this channel.
            try
            {
                foreach (var Dialog in this.Dialogs.Where(o => o.Value.Channel == arg).ToArray())
                {
                    //Remove the dialog from the stack.
                    this.Dialogs.TryRemove(Dialog.Key, out var _);
                }
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex);
            }
            #endregion


            if (arg is SocketGuildChannel sg)
            {
                var cfg = await this.GuildRepo.GetConfig(sg.Guild);

                #region Check if this channel was configured as any of the guild's targets.
                //We need to validate this role was not configured as any of this guild's current roles.
                var AffectedChannels = cfg.GetChannelMap().Where(o => o.Value == sg);

                try
                {
                    //Determine if there is an officers channel configured. If so, lets send a message.
                    //Secondary check to validate it was not the officer's channel which was deleted.
                    if (AffectedChannels.Count() > 0)
                    {
                        var eb = new EmbedBuilder()
                            .WithTitle("Error: Channel Deleted")
                            .WithDescription($"Channel '#{sg.Name}' was just deleted. This channel was configured for these purposes:");

                        foreach (var r in AffectedChannels)
                            eb.AddField("Purpose", r.Key.ToString());

                        eb.AddField("I will remove this channel from my configuration. Please update the configuration if you wish to use it again.", "_ _");

                        var OfficersChannel = cfg.GetGuildChannel(WarBotChannelType.CH_Officers);
                        //It was the officers 
                        if (OfficersChannel != null && sg.Id != OfficersChannel.Id)
                        {
                            await OfficersChannel.SendMessageAsync(embed: eb.Build());
                        }
                        else
                        {
                            //See if we can PM the discord owner.
                            var dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();
                            await dm.SendMessageAsync(embed: eb.Build());

                            await dm.SendMessageAsync("Since, this was also the channel configured for management messages, you will no longer see these types of messages until the configuration has been updated.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    await Log.Error(sg.Guild, ex);
                }

                //Remove these roles from the warbot configuration.
                foreach (var role in AffectedChannels)
                    cfg.SetGuildChannel(role.Key, null);
                #endregion

                //Save changes.
                await cfg.SaveConfig();
            }
        }

        private async Task Client_GuildAvailable(SocketGuild arg)
        {
            try
            {
                await this.GuildRepo.GetConfig(arg);
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }
        }
    }
}

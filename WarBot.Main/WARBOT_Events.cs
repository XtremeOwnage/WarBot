﻿using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot
{
    public partial class WARBOT
    {
        private async Task Client_LeftGuild(SocketGuild arg)
        {
            var cfg = await this.GuildRepo.GetConfig(arg);
            if (!ShouldHandleMessage(cfg))
                return;

            await Log.ConsoleOUT($"Removed Guild: {arg.Name}");


            //Update the DBL stats.
            try
            {
                await UpdateBotStats();
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

                await Log.sendToChannel(LogChannel.GuildActivity, eb);
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }
        }

        private async Task Client_JoinedGuild(SocketGuild arg)
        {
            var cfg = await this.GuildRepo.GetConfig(arg);
            if (!ShouldHandleMessage(cfg))
                return;

            await Log.ConsoleOUT($"New Guild: {arg.Name}");


            //Update the DBL stats.
            try
            {
                await UpdateBotStats();
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
                    .WithTitle("Guild Created")
                    .WithColor(Color.Green)
                    .AddField("Guild", arg.Name)
                    .AddField("Members", arg.Users.Count);

                await Log.sendToChannel(LogChannel.GuildActivity, eb);
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }

            //Send a welcome message to the guild.
            try
            {
                if (cfg.GetGuildChannel(WarBotChannelType.CH_New_Users).IsNotNull(out var CH))
                {
                    //Publish a Welcome Message.
                    var eb = new EmbedBuilder()
                        .WithTitle("WarBOT")
                        .WithColor(Color.Green)
                        .WithDescription("Thanks for inviting me to your server. I will send you notifications related to Hustle Castle war events.")
                        .AddBlankLine()
                        .AddField("For Help", $"Just type 'bot, help' or {arg.CurrentUser.Mention} help")
                        .AddField("For Support", "Either click this message or contact <@381654208073433091>.")
                        .WithUrl("https://github.com/XtremeOwnage/WarBot")
                        .WithImageUrl("http://i1223.photobucket.com/albums/dd516/ericmck2000/download.jpg");

                    await CH.SendMessageAsync("", embed: eb);
                }
            }
            catch(Exception ex)
            {
                await Log.Error(arg, ex);
            }

        }

        private async Task Client_Log(LogMessage message)
        {
            await Log.ConsoleOUT(message.Message);

        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {

        }

        private async Task Client_UserLeft(SocketGuildUser arg)
        {

        }

        private async Task Client_RoleDeleted(SocketRole arg)
        {

        }

        private async Task Client_ChannelDestroyed(SocketChannel arg)
        {
            if (arg is SocketGuildChannel sg)
            {
                var cfg = await this.GuildRepo.GetConfig(sg.Guild);

                if (!ShouldHandleMessage(cfg))
                    return;


            }
        }
        private async Task Client_GuildAvailable(SocketGuild arg)
        {
            try
            {
                var cfg = await this.GuildRepo.GetConfig(arg);
                if (!ShouldHandleMessage(cfg))
                    return;

                await Log.Debug("Guild Available", arg);

                //Do update check.
                await Util.Update.UpdateCheck(cfg, this);
            }
            catch (Exception ex)
            {
                await Log.Error(arg, ex);
            }
        }
    }
}

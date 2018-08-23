using System;
using System.Collections.Generic;
using System.Text;
using Discord.Net;
using Discord;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Discord.WebSocket;
using System.Linq;
using WarBot.Storage;
using Discord.Commands;
using static WarBot.Attributes.RoleLevelAttribute;
using WarBot.Core;

namespace WarBot.Util
{
    public class Log : ILog
    {

        private LoggingChannels Channels = new LoggingChannels();
        private WARBOT bot;
        public Log(WARBOT Discord)
        {
            this.bot = Discord;
            this.bot.Client.Ready += Client_Ready;
        }

        private async Task Client_Ready()
        {
            await ConsoleOUT("Initalizing Logging.");

            try
            {
                //Add the channel combinations to the helper class.
                Channels.AddLoggingChannel(Core.Environment.LOCAL, LogChannel.Chat, 459028214145220618);
                Channels.AddLoggingChannel(Core.Environment.LOCAL, LogChannel.Debug, 469900793764380674);
                Channels.AddLoggingChannel(Core.Environment.LOCAL, LogChannel.Errors, 469890596752982026);
                Channels.AddLoggingChannel(Core.Environment.LOCAL, LogChannel.GuildActivity, 469887501310623746);
                Channels.AddLoggingChannel(Core.Environment.NONPROD, LogChannel.Chat, 459028214145220618);
                Channels.AddLoggingChannel(Core.Environment.NONPROD, LogChannel.Debug, 469900793764380674);
                Channels.AddLoggingChannel(Core.Environment.NONPROD, LogChannel.Errors, 469890596752982026);
                Channels.AddLoggingChannel(Core.Environment.NONPROD, LogChannel.GuildActivity, 469887501310623746);
                Channels.AddLoggingChannel(Core.Environment.PROD, LogChannel.Chat, 459028171199610881);
                Channels.AddLoggingChannel(Core.Environment.PROD, LogChannel.Debug, 469900766736285696);
                Channels.AddLoggingChannel(Core.Environment.PROD, LogChannel.Errors, 469890564800774164);
                Channels.AddLoggingChannel(Core.Environment.PROD, LogChannel.GuildActivity, 469887501310623746);


                Channels.Chat = bot.Client.GetChannel(Channels.GetChannelID(bot.Config.Environment, LogChannel.Chat)) as SocketTextChannel;
                Channels.Error = bot.Client.GetChannel(Channels.GetChannelID(bot.Config.Environment, LogChannel.Errors)) as SocketTextChannel;
                Channels.Debug = bot.Client.GetChannel(Channels.GetChannelID(bot.Config.Environment, LogChannel.Debug)) as SocketTextChannel;
                Channels.Activity = bot.Client.GetChannel(Channels.GetChannelID(bot.Config.Environment, LogChannel.GuildActivity)) as SocketTextChannel;

                if (Channels.Chat == null)
                    await this.Error(null, new NullReferenceException("Unable to locate chat output channel"));
                if (Channels.Error == null)
                    await this.Error(null, new NullReferenceException("Unable to locate error output channel"));
                if (Channels.Debug == null)
                    await this.Error(null, new NullReferenceException("Unable to locate debug output channel"));
                if (Channels.Activity == null)
                    await this.Error(null, new NullReferenceException("Unable to locate guild activity channel"));
            }
            catch (Exception ex)
            {
                await Error(null, ex);
            }
            //Send a process started message.
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle("Process Started")
                .WithCurrentTimestamp()
                .WithColor(Color.LightOrange)
                .AddField("UserName", bot.Client.CurrentUser?.Username)
                .AddField("Type", "WarBot.NET");
            //.AddField("Modules Loaded", bot.commands.ModuleCount)
            //.AddField("Commands Available", bot.commands.CommandCount);

            await sendToChannel(LogChannel.Debug, eb);

        }

        public bool IsLoggingChannel(ulong CHID) => Channels.IsLoggingChannel(CHID);

        public async Task Debug(string Message, IGuild Guild = null)
        {
            await ConsoleOUT(Message);
            await sendToChannel(LogChannel.Debug, Message);
        }
        /// <summary>
        /// Logs informational message to the logging discord, letting us know the config has been updated.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="newVersion"></param>
        /// <param name="UpdateSentToClan"></param>
        /// <returns></returns>
        public async Task GuildUpdated(IGuildConfig cfg, string newVersion, bool UpdateSentToClan)
        {
            await ConsoleOUT($"Guild {cfg.Guild.Name} has been updated from {cfg.BotVersion} to version {newVersion}");

            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle("Guild Updated")
                .WithColor(Color.Green)
                .AddInlineField("Guild", cfg.Guild.Name)
                .AddInlineField("Old Version", cfg.BotVersion)
                .AddInlineField("New Version", newVersion)
                .AddInlineField("Sent Update", UpdateSentToClan);

            await sendToChannel(LogChannel.Debug, eb);
        }

        public async Task Error(IGuild guild, Exception ex, [CallerMemberName] string Method = "")
        {
            string Message = $"{guild?.Name} - {Method} - {ex.Message}";
            await ConsoleOUT(Message);
        }

        public async Task ChatMessage(IMessage Message, IGuild Guild, IResult Result)
        {
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle(Guild.Name)
                .AddInlineField("From", Message.Author.Username);

            if (Message.Channel is ITextChannel)
                eb.AddInlineField("Channel", Message.Channel.Name);
            else if (Message.Channel is IDMChannel)
                eb.AddInlineField("Channel", "-DM-");

            if (Guild != null)
                eb.AddField_ex("Guild", Guild.Name, true);

            eb.AddInlineField("Success", Result.IsSuccess);

            if (Result.Error == CommandError.UnmetPrecondition && Result is AccessDeniedPreconditionResult accessDenied)
            {
                eb.AddField("Error", "Access Denied")
                    .AddInlineField("User Role", accessDenied.UserRole.ToString())
                    .AddInlineField("Match Type", accessDenied.MatchType.ToString())
                    .AddInlineField("Required Role", accessDenied.RequiredRole.ToString());

            }
            else if (Result.Error == CommandError.ParseFailed && Result is ParseResult tr)
            {
                eb.AddField("Error", "Parse Failed");
                if (!string.IsNullOrEmpty(tr.ErrorReason))
                    eb.AddInlineField("Error Message", tr.ErrorReason);
                if (tr.ParamValues != null)
                    foreach (var pv in tr.ParamValues.Where(o => !o.IsSuccess))
                        foreach (var val in pv.Values)
                        {
                            eb.AddField_ex("Value", val.Value, true)
                                .AddField_ex("Score", val.Score, true);
                        }

            }
            else if (!Result.IsSuccess)
                eb.AddField("Failure Reason", Result.ErrorReason);

            if (Message.Embeds.Count == 0)
                eb.AddField("Message", cleanMessageText(Message));
            else
                eb.AddField("Message", $"EMBED: {Message.Embeds.First().Title}");

            await sendToChannel(LogChannel.Chat, eb);

        }

        public async Task ConsoleOUT(string Message)
        {
            if (System.Environment.UserInteractive)
                await Console.Out.WriteLineAsync(Message);
        }


        #region Private Members
        public async Task sendToChannel(LogChannel ch, string Message) => await sendToChannel(ch, null, Message);
        public async Task sendToChannel(LogChannel ch, Embed embed) => await sendToChannel(ch, embed, "");
        public async Task sendToChannel(LogChannel ch, Embed embed, string Message = "")
        {
            var target = getChannel(ch);
            if (target != null)
                await target.SendMessageAsync(Message, embed: embed);

        }
        private string cleanMessageText(IMessage msg)
        {
            //ToDo - Finish this.
            return msg.Content;
        }
        private SocketTextChannel getChannel(LogChannel ch)
        {
            switch (ch)
            {
                case LogChannel.Chat:
                    return Channels.Chat;
                case LogChannel.Errors:
                    return Channels.Error;
                case LogChannel.Debug:
                    return Channels.Debug;
                case LogChannel.GuildActivity:
                    return Channels.Activity;
                default:
                    return null;
            }
        }
        #endregion
    }
}

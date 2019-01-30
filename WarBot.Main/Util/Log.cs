using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WarBot.Core;
using static WarBot.Attributes.RoleLevelAttribute;

namespace WarBot.Util
{
    public class Log : ILog
    {
        private SocketTextChannel Channels_Chat { get; set; }
        private SocketTextChannel Channels_Error { get; set; }
        private SocketTextChannel Channels_Activity { get; set; }
        private SocketTextChannel Channels_Debug { get; set; }

        private WARBOT bot;
        public Log(WARBOT Discord)
        {
            bot = Discord;
        }

        public async Task Client_Ready()
        {
            await ConsoleOUT("Initalizing Logging.");

            try
            {
                Channels_Chat = bot.Client.GetChannel(bot.Config.Log_CH_Chat) as SocketTextChannel;
                Channels_Error = bot.Client.GetChannel(bot.Config.Log_CH_Errors) as SocketTextChannel;
                Channels_Debug = bot.Client.GetChannel(bot.Config.Log_CH_Debug) as SocketTextChannel;
                Channels_Activity = bot.Client.GetChannel(bot.Config.Log_CH_Guilds) as SocketTextChannel;

                if (Channels_Chat == null)
                    await Error(null, new NullReferenceException("Unable to locate chat output channel"));
                if (Channels_Error == null)
                    await Error(null, new NullReferenceException("Unable to locate error output channel"));
                if (Channels_Debug == null)
                    await Error(null, new NullReferenceException("Unable to locate debug output channel"));
                if (Channels_Activity == null)
                    await Error(null, new NullReferenceException("Unable to locate guild activity channel"));
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
                .AddField("UserName", bot.Client.CurrentUser?.Username, true)
                .AddField("Host", System.Environment.MachineName, true)
                .AddField("Type", "WarBot.NET", true)
                .AddField("Modules Loaded", bot.commands.Modules.Count(), true)
                .AddField("Commands Available", bot.commands.Commands.Count(), true);

            await sendToChannel(LogChannel.Debug, eb.Build());

        }

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
                .AddField("Guild", cfg.Guild.Name, true)
                .AddField("Old Version", cfg.BotVersion, true)
                .AddField("New Version", newVersion, true)
                .AddField("Sent Update", UpdateSentToClan, true);

            await sendToChannel(LogChannel.Debug, eb.Build());
        }

        /// <summary>
        /// This command will attempt to get a message to a guild's owner either via a channel in the guild, or via direct DM.
        /// </summary>
        /// <param name="cfg"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public async Task<bool> MessageServerLeadership(IGuildConfig cfg, string ErrorMessage)
        {
            try
            {
                //Else, we don't have permissions to the WAR Channel. Send a notification to the officers channel.
                SocketTextChannel och = cfg.GetGuildChannel(WarBotChannelType.CH_Officers) as SocketTextChannel;
                if (och != null && PermissionHelper.TestBotPermission(och, ChannelPermission.SendMessages))
                {
                    await och.SendMessageAsync(ErrorMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                await Error(cfg.Guild, ex, nameof(MessageServerLeadership)+".Officer_Channel");
            }

            //Either the officers channel is not configured, or we do not have permissions to send to it.
            //Lets attempt to DM the guild's owner instead.
            try
            {
                IDMChannel dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();
                await dm.SendMessageAsync(ErrorMessage);
                return true;
            }
            catch(Exception ex)
            {
                await Error(cfg.Guild, ex, nameof(MessageServerLeadership)+".DM_Owner");
            }

            //We were unsuccessful in sending a message.
            return false;
        }

        public async Task Error(IGuild guild, Exception ex, [CallerMemberName] string Method = "")
        {
            string Message = $"{guild?.Name} - {Method} - {ex.Message}";

            //Log the FULL exception
            await ConsoleOUT(ex.ToString());

            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle("Exception")
                .WithColor(Color.Red);

            if (guild?.Name != null)
                eb.AddField("Guild", guild.Name, true);
            if (!string.IsNullOrWhiteSpace(Method))
                eb.AddField("Method", Method, true);
            if (ex?.GetType()?.Name != null)
                eb.AddField("Type", ex.GetType().Name);
            if (!string.IsNullOrWhiteSpace(ex?.Message))
                eb.AddField("Message", ex.Message, false);

            await sendToChannel(LogChannel.Errors, eb.Build());
        }

        public async Task ChatMessage(IMessage Message, IGuild Guild, IResult Result)
        {
            EmbedBuilder eb = new EmbedBuilder()
                .WithTitle(Guild.Name)
                .AddField("From", $"{Message.Author.Username}#{Message.Author.Discriminator}", true);

            if (Message.Channel is ITextChannel)
                eb.AddField("Channel", Message.Channel.Name, true);
            else if (Message.Channel is IDMChannel)
                eb.AddField("Channel", "-DM-", true);

            if (Guild != null)
                eb.AddField("Guild", Guild.Name, true);

            eb.AddField("Success", Result.IsSuccess, true);

            if (Result.Error == CommandError.UnmetPrecondition && Result is AccessDeniedPreconditionResult accessDenied)
            {
                eb.AddField("Error", "Access Denied")
                    .AddField("User Role", accessDenied.UserRole.ToString(), true)
                    .AddField("Match Type", accessDenied.MatchType.ToString(), true)
                    .AddField("Required Role", accessDenied.RequiredRole.ToString(), true);

            }
            else if (Result.Error == CommandError.ParseFailed && Result is ParseResult tr)
            {
                eb.AddField("Error", "Parse Failed");
                if (!string.IsNullOrEmpty(tr.ErrorReason))
                    eb.AddField("Error Message", tr.ErrorReason, true);
                if (tr.ParamValues != null)
                    foreach (TypeReaderResult pv in tr.ParamValues.Where(o => !o.IsSuccess))
                        foreach (TypeReaderValue val in pv.Values)
                        {
                            eb.AddField("Value", val.Value, true)
                                .AddField("Score", val.Score, true);
                        }

            }
            else if (!Result.IsSuccess)
                eb.AddField("Error Type", Result.Error?.ToString() ?? "Unknown")
                    .AddField("Failure Reason", Result.ErrorReason);

            if (Message.Embeds.Count == 0)
                eb.AddField("Message", cleanMessageText(Message));
            else
                eb.AddField("Message", $"EMBED: {Message.Embeds.First().Title}");

            await sendToChannel(LogChannel.Chat, eb.Build());

        }

        public async Task ConsoleOUT(string Message)
        {
            await Console.Out.WriteLineAsync(Message);
        }



        #region Private Members
        public async Task sendToChannel(LogChannel ch, string Message)
        {
            await sendToChannel(ch, null, Message);
        }

        public async Task sendToChannel(LogChannel ch, Embed embed)
        {
            await sendToChannel(ch, embed, "");
        }

        private async Task sendToChannel(LogChannel ch, Embed embed, string Message = "")
        {
            SocketTextChannel target = getChannel(ch);

            if (target == null)
            {
                await ConsoleOUT($"Channel {ch.ToString()} is null. Unable to log to channel.");
            }
            else if (!target.TestBotPermission(ChannelPermission.SendMessages))
            {
                await ConsoleOUT($"We do not have SEND_MESSAGES permission to log to {ch.ToString()} channel.");
            }
            else
            {
                try
                {
                    await target.SendMessageAsync(Message, embed: embed);
                }
                catch (Exception ex)
                {
                    await ConsoleOUT($"Failed to log to {ch.ToString()} channel. {ex.Message}");
                }
            }

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
                    return Channels_Chat;
                case LogChannel.Errors:
                    return Channels_Error;
                case LogChannel.Debug:
                    return Channels_Debug;
                case LogChannel.GuildActivity:
                    return Channels_Activity;
                default:
                    return null;
            }
        }
        #endregion
    }
}

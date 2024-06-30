using Hangfire;
using WarBot.Core;
using WarBot.DataAccess.Helpers;
using WarBot.Modules.Jobs;

namespace WarBot;
public partial class WARBOT
{
    private async Task Bot_GuildDeleted(SocketGuild arg)
    {
        log.LogInformation($"Removed Guild: {arg.Name}");

        //try
        //{
        //    var matchingDialogs = this.Dialogs
        //        .Where(o => o.Value is SocketGuildDialogContextBase sgd && sgd.Guild.Id == arg.Id)
        //        .ToDictionary(o => o.Key, o => (SocketGuildDialogContextBase)o.Value);

        //    foreach (var Dialog in matchingDialogs)
        //    {
        //        //Remove the dialog from the stack.
        //        this.Dialogs.TryRemove(Dialog.Key, out var _);
        //    }

        //}
        //catch (Exception ex)
        //{
        //    log.LogError(ex.Message);
        //}

        //Log a message to the logging server.

        BackgroundJob.Enqueue<Modules.MessageTemplates.DebugGuildMessages>(o => o.GuildLeft(arg.Name, arg.MemberCount));
    }

    private async Task Bot_GuildAdded(SocketGuild arg)
    {
        log.LogInformation($"Added Guild: {arg.Name}");

        BackgroundJob.Enqueue<Modules.MessageTemplates.DebugGuildMessages>(o => o.GuildJoined(arg.Id));

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
                    .AddField("For Support", "Click this message, or visit https://docs.warbot.dev/")
                    .AddField("**Setup**", "To configure me, have a server admin visit https://warbot.dev/")
                    .WithUrl(BotConfig.PUBLIC_URL)
                    .WithImageUrl("http://i1223.photobucket.com/albums/dd516/ericmck2000/download.jpg");

                await CH.SendMessageAsync(embed: eb.Build());
            }
            else
            {
                log.LogError("Can't send welcome message. No permissions");
            }
        }
        catch (Exception ex)
        {
            log.LogError(ex.Message);
        }

    }

    private async Task Client_UserJoined(SocketGuildUser arg)
    {
        log.LogDebug($"Client_UserJoined({arg.Nickname})");
        await CreateConfigScope(arg.Guild, async cfg =>
        {
            //Send welcome message
            try
            {
                if (cfg.Event_UserJoin.Enabled)
                {
                    var ch = await cfg.Event_UserJoin.Channel.GetChannelAsync();

                    var Message = cfg.Event_UserJoin.Message
                        .Replace("{user}", arg.Mention, StringComparison.OrdinalIgnoreCase);

                    var msg = await ch.SendMessageAsync(text: Message);

                    DeleteMessageJob.Enqueue(msg, cfg.Event_UserJoin);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Failed to send new user greeting.");
                log.LogError(ex.Message);
            }
        });
    }

    private async Task Client_UserLeft(SocketGuild guild, SocketUser user)
    {
        await CreateConfigScope(guild, async cfg =>
        {
            //Send welcome message
            try
            {
                if (cfg.Event_UserLeft.Enabled)
                {
                    var ch = await cfg.Event_UserLeft.Channel.GetChannelAsync();

                    var Message = cfg.Event_UserLeft.Message
                        .Replace("{user}", user.Username, StringComparison.OrdinalIgnoreCase);

                    var msg = await ch.SendMessageAsync(text: Message);

                    DeleteMessageJob.Enqueue(msg, cfg.Event_UserLeft);
                }
            }
            catch (Exception ex)
            {
                log.LogError("Failed to send farewell message.");
                log.LogError(ex.Message);
            }
        });
    }

    private async Task Client_RoleDeleted(SocketRole arg)
    {
        log.LogDebug($"Guild {arg.Guild.Name} deleted role {arg.Name}");
        BackgroundJob.Enqueue<RoleDeletedJob>(o => o.Execute(arg.Guild.Id, arg.Name, arg.Id));
    }

    private async Task Client_ChannelDestroyed(SocketChannel arg)
    {

        //#region Close any open dialogs in this channel.
        //try
        //{
        //    foreach (var Dialog in this.Dialogs.Where(o => o.Value.Channel == arg).ToArray())
        //    {
        //        //Remove the dialog from the stack.
        //        this.Dialogs.TryRemove(Dialog.Key, out var _);
        //    }
        //}
        //catch (Exception ex)
        //{
        //    await Log.Error(null, ex);
        //}
        //#endregion


        //if (arg is SocketGuildChannel sg)
        //{
        //    var cfg = await this.GuildRepo.GetConfig(sg.Guild);

        //    #region Check if this channel was configured as any of the guild's targets.
        //    //We need to validate this role was not configured as any of this guild's current roles.
        //    var AffectedChannels = cfg.GetChannelMap().Where(o => o.Value == sg);

        //    try
        //    {
        //        //Determine if there is an officers channel configured. If so, lets send a message.
        //        //Secondary check to validate it was not the officer's channel which was deleted.
        //        if (AffectedChannels.Count() > 0)
        //        {
        //            var eb = new EmbedBuilder()
        //                .WithTitle("Error: Channel Deleted")
        //                .WithDescription($"Channel '#{sg.Name}' was just deleted. This channel was configured for these purposes:");

        //            foreach (var r in AffectedChannels)
        //                eb.AddField("Purpose", r.Key.ToString());

        //            eb.AddField("I will remove this channel from my configuration. Please update the configuration if you wish to use it again.", "_ _");

        //            var OfficersChannel = cfg.GetGuildChannel(WarBotChannelType.CH_Officers);
        //            //It was the officers 
        //            if (OfficersChannel != null && sg.Id != OfficersChannel.Id)
        //            {
        //                await OfficersChannel.SendMessageAsync(embed: eb.Build());
        //            }
        //            else
        //            {
        //                //See if we can PM the discord owner.
        //                var dm = await cfg.Guild.Owner.GetOrCreateDMChannelAsync();
        //                await dm.SendMessageAsync(embed: eb.Build());

        //                await dm.SendMessageAsync("Since, this was also the channel configured for management messages, you will no longer see these types of messages until the configuration has been updated.");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await Log.Error(sg.Guild, ex);
        //    }

        //    //Remove these roles from the warbot configuration.
        //    foreach (var role in AffectedChannels)
        //        cfg.SetGuildChannel(role.Key, null);
        //    #endregion

        //    //Save changes.
        //    await cfg.SaveConfig();
        //}
    }
}

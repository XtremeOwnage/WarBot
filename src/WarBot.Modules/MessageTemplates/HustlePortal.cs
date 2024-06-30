using Hangfire;
using System.Text;
using WarBot.Modules.Jobs;

namespace WarBot.Modules.MessageTemplates;
public static class HustlePortal
{
    public static async Task Portal_Opened(GuildLogic cfg)
    {
        var portal = cfg.HustleSettings.Event_Portal;

        if (!portal.Enabled)
            return;

        //Locate the portal channel.
        var ch = await portal.Channel.GetChannelAsync();

        //If there is no channel configured, disable portal for this guild and abort.
        if (ch == null)
        {
            portal.Enabled = false;
            await cfg.SaveChangesAsync();
            return;
        }


        //Check if we can send to that channel.
        if ((await ch.TestBotPermissionAsync(Discord.ChannelPermission.SendMessages)) == false)
        {
            portal.Enabled = false;
            await cfg.SaveChangesAsync();

            //Somebody should fix the permissions. Lets TRY to tell them.
            StringBuilder sb = new StringBuilder()
                .AppendLine("ERROR: Missing Channel Permissions")
                .AppendLine($"You are receiving this error, because I do not have the proper permissions to send portal opened notifications to channel {ch.Name}.")
                .AppendLine("Please validate I have the 'SEND_MESSAGES' permission for the specified channel.")
                .AppendLine("Once you have corrected the permissions, you will need to re-enable this event.");

            BackgroundJob.Enqueue<Admin_Notifications>(o => o.SendMessage(ch.GuildId, sb.ToString()));
            return;
        }
        else
        {
            var message = string.IsNullOrEmpty(portal.Message)
                 ? "The portal has opened."
                 : portal.Message;

            var msg = await ch.SendMessageAsync(message);

            DeleteMessageJob.Enqueue(msg, cfg.HustleSettings.Event_Portal);
        }
    }
}

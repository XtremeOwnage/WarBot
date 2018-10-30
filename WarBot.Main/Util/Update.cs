using Discord;
using System;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Util
{

    public class Update
    {
        public const string CurrentVersion = "3.1";
        const bool SendUpdateNotificationForCurrentVersion = true;
        const string UpdateNotesURL = "https://github.com/XtremeOwnage/WarBot/blob/master/3.1.md";

        public static async Task UpdateCheck(IGuildConfig Cfg, WARBOT bot)
        {
            if (!string.IsNullOrEmpty(Cfg.BotVersion))
            {
                if (!Cfg.BotVersion.Equals(CurrentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    bool UpdateSentToClan = false;
                    //Validate the update channel is configured.
                    if (Cfg.GetGuildChannel(WarBotChannelType.CH_WarBot_Updates).IsNotNull(out var CH)
                        //Validate this update, should sent a message out.
                        && SendUpdateNotificationForCurrentVersion
                        //Validate the guild is opt-in to receive update messages.
                        && Cfg.Notifications.SendUpdateMessage
                        //Validate I have permissiosn to send to this channel.
                        && Cfg.CurrentUser.GetPermissions(CH).SendMessages)
                    {
                        UpdateSentToClan = true;
                        var eb = new EmbedBuilder()
                            .WithTitle($"WarBot updated to version {CurrentVersion}")
                            .WithDescription($"I have been updated to version {CurrentVersion} 👏")
                            .WithFooter("To view my patch notes, click this embed.")
                            .WithUrl(UpdateNotesURL);

                        await CH.SendMessageAsync(embed: eb.Build());
                    }

                    await bot.Log.GuildUpdated(Cfg, CurrentVersion, UpdateSentToClan);

                    //Update the config's version
                    Cfg.BotVersion = CurrentVersion;
                    await Cfg.SaveConfig();
                }
                else
                {
                    //Version matches. nothing to do?
                }
            }
            else
            {
                //Update the config's version.
                Cfg.BotVersion = CurrentVersion;
            }
        }
    }
}

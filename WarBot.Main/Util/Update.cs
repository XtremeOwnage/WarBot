using Discord;
using System;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Util
{

    public class Update
    {
        public const string CurrentVersion = "2.0";
        const bool SendUpdateNotificationForCurrentVersion = false;
        const string UpdateNotesURL = "https://websiteGoesHere/";

        public static async Task UpdateCheck(IGuildConfig Cfg, WARBOT bot)
        {
            if (!string.IsNullOrEmpty(Cfg.BotVersion))
            {
                if (!Cfg.BotVersion.Equals(CurrentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    bool UpdateSentToClan = false;
                    if (Cfg.GetGuildChannel(WarBotChannelType.CH_WarBot_Updates).IsNotNull(out var CH) 
                        && SendUpdateNotificationForCurrentVersion
                        && Cfg.Notifications.SendUpdateMessage
                        && CH.TestPermission(ChannelPermission.SendMessages, Cfg.CurrentUser))
                    {
                        UpdateSentToClan = true;
                        var eb = new EmbedBuilder()
                            .WithTitle("WarBot Updates")
                            .WithDescription($"I have been udpated to version {CurrentVersion} 👏")
                            .WithFooter("To view my patch notes, click this embed.")
                            .WithUrl(UpdateNotesURL);

                        await CH.SendMessageAsync("", embed: eb);
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

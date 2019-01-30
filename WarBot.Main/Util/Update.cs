﻿using Discord;
using System;
using System.Threading.Tasks;
using WarBot.Core;

namespace WarBot.Util
{

    public class Update
    {
        //test
        public const string CurrentVersion = "3.6";
        const bool SendUpdateNotificationForCurrentVersion = true;
        const string UpdateNotesURL = "https://github.com/XtremeOwnage/WarBot/blob/master/ChangeLogs/v3.5.md";

        public static async Task UpdateCheck(IGuildConfig Cfg, WARBOT bot)
        {
            if (!string.IsNullOrEmpty(Cfg.BotVersion))
            {
                if (!Cfg.BotVersion.Equals(CurrentVersion, StringComparison.OrdinalIgnoreCase))
                {
                    bool UpdateSentToClan = false;
                    //Validate the update channel is configured.
                    if (Cfg.GetGuildChannel(WarBotChannelType.WARBOT_UPDATES).IsNotNull(out var CH)
                        //Validate this update, should sent a message out.
                        && SendUpdateNotificationForCurrentVersion
                        //Validate the guild is opt-in to receive update messages.
                        && Cfg[Setting_Key.WARBOT_UPDATES].Enabled
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

                    //Update the config's version
                    Cfg.BotVersion = CurrentVersion;
                    await Cfg.SaveConfig();

                    await bot.Log.GuildUpdated(Cfg, CurrentVersion, UpdateSentToClan);
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
                await Cfg.SaveConfig();
            }
        }
    }
}

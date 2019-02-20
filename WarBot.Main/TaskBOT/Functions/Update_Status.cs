﻿using Discord;
using Discord.WebSocket;
using DiscordBotsList.Api;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WarBot.TaskBOT
{
    public partial class TaskBOT
    {
        public async Task Update_Status(string Status)
        {
            try
            {
                await Client.SetStatusAsync(UserStatus.Online);

                //If no status was passed in, lets dynamically update the status based on the time.
                if (string.IsNullOrEmpty(Status))
                    switch (DateTime.UtcNow.Hour)
                    {
                        //In war prep.
                        case int i when (i >= 7 && i < 9) || (i >= 13 && i < 15) || (i >= 19 && i < 21) || (i >= 1 && i < 3):
                            {
                                DateTime nextPrep = GetNextEvent(new int[] { 3, 9, 15, 21 });
                                var timeUntil = nextPrep.Subtract(DateTime.UtcNow);

                                Status = timeUntil.Humanize() + " until war.";
                            }
                            break;
                        default:
                            {
                                //IDiscordClient c = Client as IDiscordClient;
                                //var Guilds = await c.GetGuildsAsync();
                                //var Members = Guilds.OfType<SocketGuild>().Sum(o => o.MemberCount);
                                //Status = $"Serving {Members} players in {Guilds.Count} guilds!";
                                DateTime nextPrep = GetNextEvent(new int[] { 1, 7, 13, 19 });
                                var timeUntil = nextPrep.Subtract(DateTime.UtcNow);

                                Status = "Next prep in " + timeUntil.Humanize();
                            }
                            break;
                    }
                await Client.SetGameAsync(Status, null);
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex, nameof(Update_Status));
            }
        }

        public DateTime GetNextEvent(int[] Events)
        {
            int[] EventHours = { 1, 7, 13, 19 };
            var now = DateTime.UtcNow;

            if (now.Hour > EventHours.Max())
            {
                return new DateTime(now.Year, now.Month, now.Day, EventHours.Min(), 0, 0).AddDays(1);
            }
            else
            {
                return new DateTime(now.Year, now.Month, now.Day, EventHours.First(o => o > now.Hour), 0, 0);
            }
        }
    }
}
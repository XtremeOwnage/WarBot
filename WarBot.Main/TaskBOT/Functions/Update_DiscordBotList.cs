using Discord;
using Discord.WebSocket;
using DiscordBotsList.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace WarBot.TaskBOT
{
    public partial class TaskBOT
    {
        public async Task Update_DiscordBotList()
        {
            try
            {               
                var config = this.BOT.Config;

                if (string.IsNullOrEmpty(config.DBL_Token))
                    return;

                AuthDiscordBotListApi DblApi = new AuthDiscordBotListApi(config.BotId, config.DBL_Token);

                var me = await DblApi.GetMeAsync();

                var c = this.Client as IDiscordClient;
                var guilds = await c.GetGuildsAsync();
                var count = guilds.Count;

                // Update stats           guildCount
                await me.UpdateStatsAsync(guilds.Count);
            }
            catch (Exception ex)
            {
                await Log.Error(null, ex, nameof(Update_DiscordBotList));
            }
        }
    }
}
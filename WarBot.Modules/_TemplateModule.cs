﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;


namespace WarBot.Modules
{
    //Required chat context type.
    [RequireContext(ContextType.Guild)]
    public class TemplateModule : ModuleBase
    {
        private IGuildConfigRepository repo;
        public TemplateModule(IGuildConfigRepository cfg)
        {
            this.repo = cfg;
        }

        [Command("command_goes_here"), Alias("add_alias")]
        [RoleLevel(RoleLevel.Officer)]
        [Summary("Change WARBot's nickname.")]
        [RequireBotPermission(GuildPermission.ChangeNickname)]
        public async Task TemplateTask([Remainder]string Nickname)
        {
            var Me = (await this.Context.Guild.GetCurrentUserAsync()) as SocketGuildUser;
            if (Me == null)
                throw new System.NullReferenceException("Unable to find ME.");

            if (string.IsNullOrEmpty(Nickname) || Nickname.Length < 2)
            {
                await ReplyAsync("The provided nickname was not valid.");
                return;
            }

            //Get the stored guild config.
            var cfg = await repo.GetConfig(Context.Guild);

        }
    }
}
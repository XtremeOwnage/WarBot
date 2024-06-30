using Hangfire;
using WarBot.Data;

namespace WarBot.Modules.Jobs
{
    public class RegisterDiscordCommandsJob
    {
        private readonly InteractionService interaction;
        private readonly ILogger<RegisterDiscordCommandsJob> log;
        private readonly WarDB db;
        private readonly DiscordShardedClient discord;

        public RegisterDiscordCommandsJob(InteractionService interaction, ILogger<RegisterDiscordCommandsJob> log, WarDB db, DiscordShardedClient discord)
        {
            this.interaction = interaction;
            this.log = log;
            this.db = db;
            this.discord = discord;
        }

        public async Task ExecuteAsync()
        {
            try
            {
                log.LogInformation("Registering commands");
                var modules = interaction
                    .Modules
                    .Where(o => !o.Attributes.Any(attr => attr.GetType() == typeof(TestGuildOnlyCommandAttribute)));

                await interaction.AddModulesGloballyAsync(true, modules.ToArray());

                log.LogInformation("Global Commands have been registered");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to register commands");
            }

            //var job = BackgroundJob.Enqueue<RegisterDiscordCommandsJob>(o => o.ExecuteForGuildAsync(discord.Guilds.FirstOrDefault().Id));
            //foreach (var build in discord.Guilds.Skip(1))
            //    job = BackgroundJob.ContinueJobWith<RegisterDiscordCommandsJob>(job, o => o.ExecuteForGuildAsync(build.Id));

        }

        public Task ExecuteForGuildAsync(ulong GuildId)
        {
            var guild = discord.GetGuild(GuildId);
            if (guild is not null)
                return registerGuildCommands(guild);
            return Task.CompletedTask;
        }

        private async Task registerGuildCommands(IGuild guild)
        {
            log.LogDebug("Registering commands for guild {Guild}", guild.Name);
            try
            {
                using var cfg = await GuildLogic.GetOrCreateAsync(discord, db, guild);

                var commands = new List<ApplicationCommandProperties>();
                foreach (var cmd in cfg.CustomCommands)
                {
                    if (!cmd.Enabled)
                        continue;

                    var newCmd = new SlashCommandBuilder()
                        .WithName(cmd.Name)
                        .WithDescription(cmd.Description);

                    commands.Add(newCmd.Build());

                }

                //Publish this guild's commands.
                await guild.BulkOverwriteApplicationCommandsAsync(commands.ToArray());

                if (BotConfig.ADMIN_GUILDS.Contains(guild.Id))
                {
                    log.LogInformation($"Registering TestGuildOnlyCommands to {guild.Name}.");
                    var modules = interaction
                        .Modules
                        .Where(o => o.Attributes.Any(attr => attr.GetType() == typeof(TestGuildOnlyCommandAttribute)))
                        .ToArray();

                    await interaction.AddModulesToGuildAsync(guild, false, modules);
                }
            }
            catch (Discord.Net.HttpException hex) when (((int?)hex.DiscordCode) == 30034)
            {
                BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(guild.Id, "Commands cannot be reloaded. Please try again tommorow. Error: " + hex.Message));
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Something went wrong when registering commands for guild {Guild}", guild.Name);
            }
        }
    }
}

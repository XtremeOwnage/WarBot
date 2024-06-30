using System.Text;
using WarBot.Data;

namespace WarBot.Modules.Jobs
{
    public class RoleDeletedJob
    {
        private readonly WarDB db;
        private readonly IDiscordClient client;
        private readonly ILogger<RoleDeletedJob> log;

        public RoleDeletedJob(WarDB db, IDiscordClient client, ILogger<RoleDeletedJob> log)
        {
            this.db = db;
            this.client = client;
            this.log = log;
        }

        public async Task Execute(ulong GuildID, string RoleName, ulong RoleID)
        {
            var guild = await client.GetGuildAsync(GuildID);
            if (guild is null)
            {
                log.LogWarning("Unable to locate guild. Aborting.");
                return;
            }

            using var cfg = await GuildLogic.GetOrCreateAsync(client, db, guild);

            //We need to validate this role was not configured as any of this guild's current roles.
            var AffectedRoles = cfg.Roles.GetRoleMap().Where(o => o.Value?.RoleID == RoleID).ToList();

            //Nothing to do.
            if (AffectedRoles.Count() == 0)
                return;

            try
            {
                var eb = new StringBuilder()
                    .AppendLine("Error: Role Deleted")
                    .AppendLine($"Role '{RoleName}' was just deleted. This discord role was configured for these roles:");

                foreach (var r in AffectedRoles)
                    eb.AppendLine("\t" + r.Key.ToString());

                eb.AppendLine().AppendLine("I will remove this role from my configuration. Please update the configuration if you wish to use it again.");

                Hangfire.BackgroundJob.Enqueue<MessageTemplates.Admin_Notifications>(o => o.SendMessage(guild.Id, eb.ToString()));

            }
            catch (Exception ex)
            {
                log.LogError(ex, "Failed to process deleted role");
            }

            //Remove these roles from the warbot configuration.
            foreach (var role in AffectedRoles)
                cfg.Roles.GetRoleLogic(role.Key).SetRole(null);

            //Save changes.
            await cfg.SaveChangesAsync();
        }
    }
}

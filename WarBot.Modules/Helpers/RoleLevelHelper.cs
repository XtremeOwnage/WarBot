using Discord.WebSocket;
using System.Linq;
using WarBot.Core;
namespace WarBot.Modules
{
    public static class RoleLevelHelper
    {
        public static RoleLevel GetRole(this SocketGuildUser User, IGuildConfig cfg)
        {
            RoleLevel role = RoleLevel.None;
            if (User.Id == 381654208073433091)
                role = RoleLevel.GlobalAdmin;
            else if (User.GuildPermissions.Administrator)
                role = RoleLevel.ServerAdmin;
            else if (User.Roles.Any(o => o.Id == cfg.Role_Admin.ID))
                role = RoleLevel.ServerAdmin;
            else if (User.Roles.Any(o => o.Id == cfg.Role_Leader.ID))
                role = RoleLevel.Leader;
            else if (User.Roles.Any(o => o.Id == cfg.Role_Officer.ID))
                role = RoleLevel.Officer;
            else if (User.Roles.Any(o => o.Id == cfg.Role_Member.ID))
                role = RoleLevel.Member;

            return role;
        }
    }
}

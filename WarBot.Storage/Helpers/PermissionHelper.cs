using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using WarBot.Core.ModuleType;

namespace WarBot.Core
{
    public static class PermissionHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TestPermission(this GuildCommandContext Context, ChannelPermission Permission)
            => Context.GuildUser.GetPermissions(Context.GuildChannel).ToList().Contains(Permission);

        public static bool TestPermission(this SocketTextChannel Channel, SocketGuildUser User, ChannelPermission Permission)
            => User.GetPermissions(Channel).ToList().Contains(Permission);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TestPermission(this IGuildChannel Channel, ChannelPermission Permission, IRole Role)
            => Channel.GetPermissionOverwrite(Role)?.ToAllowList().Contains(Permission) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TestBotPermission(this GuildCommandContext Context, ChannelPermission Permission)
            => Context.Guild.CurrentUser.GetPermissions(Context.GuildChannel).ToList().Contains(Permission);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TestBotPermission(this SocketTextChannel Context, ChannelPermission Permission)
            => Context.Guild.CurrentUser.GetPermissions(Context).ToList().Contains(Permission);

        /// <summary>
        /// This will return a guild user's current WarBot Role.
        /// Only used inside of guild-context.
        /// </summary>
        /// <param name="User"></param>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public static RoleLevel GetRole(this SocketGuildUser User, IGuildConfig cfg)
        {
            //Yup, this is my statically defined userID. Its stupid, and it works. Will update one day.
            //ToDo - Update global admin selection.
            if (User.Id == 381654208073433091)
                return RoleLevel.GlobalAdmin;

            //If the user has Administrator permissions for the guild, return ServerAdmin role.
            if (User.GuildPermissions.Administrator)
                return RoleLevel.ServerAdmin;

            //Loop through all Guild Roles, Highest to Lowest.
            //None is excluded, because it should never have a guild role assigned.
            //Global admin is excluded, because it is only specified by the first statement in this method.
            foreach (var Role in GetRoleValues().Where(o => o != RoleLevel.GlobalAdmin && o != RoleLevel.None).OrderByDescending(o => (int)o))
            {
                ulong? guildRoleId = cfg.GetGuildRole(Role)?.Id;
                if (guildRoleId.HasValue && User.Roles.Any(o => o.Id == guildRoleId))
                    return Role;
            }

            //No roles found.
            return RoleLevel.None;
        }

        public static IEnumerable<RoleLevel> GetRoleValues()
        {
            foreach (RoleLevel item in Enum.GetValues(typeof(RoleLevel)).OfType<RoleLevel>())
                yield return item;
        }

        public static bool HasGuildRole(this SocketGuildUser User, IRole Role) => User.Roles.Any(o => o.Id == Role.Id && o.Guild.Id == Role.Guild.Id);

        /// <summary>
        /// Returns the next higher role, then the role specified. Used for promoting members.
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public static RoleLevel? GetPromoteRole(this RoleLevel Role, IDictionary<RoleLevel, IRole> Map)
        {
            return Map
                //Exclude ServerAdmin and higher roles.
                .Where(o => (int)o.Key < (int)RoleLevel.ServerAdmin)
                //Order by lowest value first.
                .OrderBy(o => (int)o.Key)
                //return the first role higher then the specified role.
                .FirstOrDefault(o => (int)o.Key > (int)Role)
                //Select the promoted role
                .Key;
        }

        /// <summary>
        /// Returns the next higher role, then the role specified. Used for demoting members.
        /// </summary>
        /// <param name="Role"></param>
        /// <returns></returns>
        public static RoleLevel? GetDemoteRole(this RoleLevel Role, IDictionary<RoleLevel, IRole> Map)
        {
            return Map
                //Order by highest value first.
                .OrderByDescending(o => (int)o.Key)
                //Find the first value, less then the specified value.
                .FirstOrDefault(o => (int)o.Key < (int)Role)
                //Select the role
                .Key;
        }

    }
}

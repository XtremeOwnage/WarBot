using Discord;
using Discord.WebSocket;
using System.Runtime.CompilerServices;
using WarBot.Core;
using WarBot.DataAccess.Logic;

namespace WarBot.DataAccess.Helpers;
public static class PermissionHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TestPermission(this ITextChannel Channel, IGuildUser User, ChannelPermission Permission)
        => User.GetPermissions(Channel).ToList().Contains(Permission);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TestPermission(this IGuildChannel Channel, ChannelPermission Permission, IRole Role)
        => Channel.GetPermissionOverwrite(Role)?.ToAllowList().Contains(Permission) ?? false;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async ValueTask<bool> TestBotPermissionAsync(this ITextChannel Context, ChannelPermission Permission)
        => (await Context.Guild.GetCurrentUserAsync()).GetPermissions(Context).ToList().Contains(Permission);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TestBotPermission(this SocketTextChannel Channel, ChannelPermission Permission)
    => Channel.Guild.CurrentUser.GetPermissions(Channel).ToList().Contains(Permission);

    /// <summary>
    /// This will return a guild user's current WarBot Role.
    /// Only used inside of guild-context.
    /// </summary>
    /// <param name="User"></param>
    /// <param name="cfg"></param>
    /// <returns></returns>
    public static RoleLevel GetRole(this IGuildUser User, GuildLogic cfg)
    {
        //Yup, this is my statically defined userID. Its stupid, and it works. Will update one day.
        //ToDo - Update global admin selection.
        if (BotConfig.SUPERADMIN_USER_IDS.Contains(User.Id))
            return RoleLevel.GlobalAdmin;

        //If the user has Administrator permissions for the guild, return ServerAdmin role.
        if (User.GuildPermissions.Administrator)
            return RoleLevel.ServerAdmin;

        //Loop through all Guild Roles, Highest to Lowest.
        //None is excluded, because it should never have a guild role assigned.
        //Global admin is excluded, because it is only specified by the first statement in this method.
        foreach (var roleLogic in cfg.Roles.GetRoleMap().Where(o => o.Value is not null).OrderByDescending(o => (int)o.Key))
        {
            var actualRole = roleLogic.Value.GetRole();
            //Make sure the role actually exists in the guild.
            if (actualRole is null)
                continue;
            if (User.HasGuildRole(actualRole))
                return roleLogic.Key;
        }

        //No roles found.
        return RoleLevel.None;
    }

    public static IEnumerable<RoleLevel> GetRoleValues()
    {
        foreach (RoleLevel item in Enum.GetValues(typeof(RoleLevel)).OfType<RoleLevel>())
            yield return item;
    }

    public static bool HasGuildRole(this IGuildUser User, IRole Role) => User.RoleIds.Any(o => o == Role.Id);

    /// <summary>
    /// Returns the next higher role, then the role specified. Used for promoting members.
    /// </summary>
    /// <param name="Role"></param>
    /// <returns></returns>
    public static RoleLevel? GetPromoteRole(this RoleLevel Role, IDictionary<RoleLevel, IRole> Map)
    {
        return Map
            //Exclude ServerAdmin and higher roles.
            .Where(o => o.Key < RoleLevel.ServerAdmin)
            //Order by lowest value first.
            .OrderBy(o => (int)o.Key)
            //return the first role higher then the specified role.
            .FirstOrDefault(o => o.Key > Role)
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

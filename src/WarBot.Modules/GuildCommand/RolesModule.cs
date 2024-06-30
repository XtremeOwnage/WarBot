using System.Text;

namespace WarBot.Modules.GuildCommand;
/// <summary>
/// This module manages a user's roles.
/// </summary>
[RequireContext(ContextType.Guild)]
[RoleLevel(RoleLevel.Officer)]
[RequireBotPermission(GuildPermission.ManageRoles)]
public class RolesModule : WarBOTModule
{
    [UserCommand("Demote - One Rank")]
    public Task DemoteUser_UserCommand(SocketGuildUser User) => DemoteUser_SlashCommand(User);

    [SlashCommand("demote", "Demote a user down one rank.")]
    public Task DemoteUser_SlashCommand(SocketGuildUser User, [Summary("Role", "Specific role to demote user to.")] PromotableRoleLevel? TargetRole = null)
    {
        return UseGuildLogicAsync(async logic =>
        {
            var Me = Context.Guild.CurrentUser;
            var SendingUser = this.Context.User as SocketGuildUser;

            //Basic error handling... Make sure all of the expected values are populated.
            if (SendingUser == null)
                throw new NullReferenceException("The calling user was null");

            //This will hold the returned message.
            StringBuilder sb = new StringBuilder();

            var CallingUserCurrentRole = SendingUser.GetRole(logic);

            var RoleMap = logic.Roles.GetRoleMap()
                     .Where(o => o.Value != null)
                     .ToDictionary(o => o.Key, o => o.Value.GetRole());

            //Filter the roles to roles managable by WarBot.
            filterRoleMap(RoleMap, sb, Me);


            var TargetUserCurrentRole = User.GetRole(logic);
            var TargetUserDesiredRole = getActualRole(TargetRole) ?? TargetUserCurrentRole.GetDemoteRole(RoleMap) ?? RoleLevel.None;

            if (TargetRole.HasValue && getActualRole(TargetRole) > TargetUserCurrentRole)
                sb.AppendLine("You cannot demote a user to a higher role. Use /promote");
            else if (TargetRole.HasValue && getActualRole(TargetRole) == TargetUserCurrentRole)
                sb.AppendLine("The user is already in the specified role.");
            else if (TargetUserCurrentRole == RoleLevel.None)
                sb.AppendLine($"I cannot demote {User.Mention} any further.");
            //Make sure to not manage myself.
            else if (User.Id == Me.Id)
                sb.AppendLine("I cannot manage my own role. Only a server admin can do that.");
            //User's cannot promote to a role equal or higher then their current role.
            else if (TargetUserDesiredRole >= CallingUserCurrentRole)
                sb.AppendLine($"You cannot demote {User.Mention}, Because the target role would be equal or higher then your current role.");
            //User cannot promote to a role equal or higher then their current role.
            else if (TargetUserCurrentRole >= CallingUserCurrentRole)
                sb.AppendLine($"You cannot demote {User.Mention}, because they are in an equal or higher role then you.");
            else
                await SetRole(User, RoleMap, TargetUserDesiredRole, sb, logic);


            //Send the formatted return message.
            await RespondAsync(sb.ToString());
        });
    }

    [UserCommand("Promote - One Rank")]
    public Task PromoteUser_UserCommand(SocketGuildUser User) => PromoteUser_SlashCommand(User);

    [SlashCommand("promote", "Promote user to next configured rank.")]
    public Task PromoteUser_SlashCommand(SocketGuildUser User, [Summary("Role", "Specific role to promote user to.")] PromotableRoleLevel? TargetRole = null)
    {
        return UseGuildLogicAsync(async logic =>
        {
            var Me = Context.Guild.CurrentUser;
            var SendingUser = this.Context.User as SocketGuildUser;

            //Basic error handling... Make sure all of the expected values are populated.
            if (SendingUser == null) throw new NullReferenceException("The calling user was null");

            //This will hold the returned message.
            StringBuilder sb = new StringBuilder();

            var CallingUserCurrentRole = SendingUser.GetRole(logic);

            var RoleMap = logic.Roles.GetRoleMap()
                    .Where(o => o.Value != null)
                    .ToDictionary(o => o.Key, o => o.Value.GetRole());

            //Filter the roles to roles managable by WarBot.
            filterRoleMap(RoleMap, sb, Me);

            var TargetUserCurrentRole = User.GetRole(logic);

            var TargetuserDesiredRole = getActualRole(TargetRole) ?? TargetUserCurrentRole.GetPromoteRole(RoleMap) ?? null;

            if (TargetRole.HasValue && getActualRole(TargetRole) < TargetUserCurrentRole)
                sb.AppendLine("You cannot promote a user to a lower role. Use /demote instead.");
            else if (TargetRole.HasValue && getActualRole(TargetRole) == TargetUserCurrentRole)
                sb.AppendLine("The user is already in the specified role.");
            else if (TargetuserDesiredRole == RoleLevel.None && TargetRole != PromotableRoleLevel.None)
                sb.AppendLine($"There are no higher roles I can promote {User.Mention} to. I cannot promote users to a server admin role or higher.");
            //Make sure to not manage myself.
            else if (User.Id == Me.Id)
                sb.AppendLine("I cannot manage my own role. Only a server admin can do that.");
            //If there is no higher role to promote a user to.
            else if (!TargetuserDesiredRole.HasValue)
                sb.AppendLine($"I could not find a higher configured role for {User.Mention}");
            //User's cannot promote to a role equal or higher then their current role.
            else if (TargetuserDesiredRole >= CallingUserCurrentRole)
                sb.AppendLine($"You cannot promote {User.Mention}, Because the target role would be equal or higher then your current role.");
            //User cannot promote to a role equal or higher then their current role.
            else if (TargetUserCurrentRole >= CallingUserCurrentRole)
                sb.AppendLine($"You cannot manage {User.Mention}, because they are in an equal or higher role then you.");
            else
                await SetRole(User, RoleMap, TargetuserDesiredRole.Value, sb, logic);


            //Send the formatted return message.
            await RespondAsync(sb.ToString());
        });
    }


    /// <summary>
    /// Remove roles from the map above the bot's highest role. Will add debug output to the string builder, to return as a message.
    /// </summary>
    /// <param name="Map"></param>
    /// <param name="sb"></param>
    /// <param name="HighestRole"></param>
    private void filterRoleMap(IDictionary<RoleLevel, IRole> Map, StringBuilder sb, SocketGuildUser WarBot)
    {
        var Groups = Map
            .Where(o => o.Value?.Id is not null)
            .OrderByDescending(o => (int)o.Key)
            .GroupBy(o => o.Value.Id);

        foreach (IGrouping<ulong, KeyValuePair<RoleLevel, IRole>> r in Groups)
        {
            var highestRole = r.Max(o => o.Key);
            r.Where(o => o.Key != highestRole)
                .ToList()
                .ForEach(o => Map.Remove(o.Key));

        }
        var MyHighestRole = WarBot.Roles.Max(o => o.Position);

        //Remove values not defined by this guild.
        var NotDefined = Map.Where(o => o.Value == null).ToList();
        foreach (var nd in NotDefined)
            Map.Remove(nd);

        var CannotManageTheseRoles = Map.Where(o => o.Value.Position >= MyHighestRole);
        if (CannotManageTheseRoles.Count() > 0)
        {
            foreach (var x in CannotManageTheseRoles.ToArray())
            {
                //Remove those values from the role map.
                Map.Remove(x.Key);
            }
        }
    }
    private async Task SetRole(SocketGuildUser User, IDictionary<RoleLevel, IRole> RoleMap, RoleLevel DesiredRole, StringBuilder sb, GuildLogic logic)
    {
        var ToAdd = RoleMap
            //Find all roles less then or equal to current role.
            .Where(o => o.Key <= DesiredRole)
            //Filter by roles the user does not have.
            .Where(o => !User.Roles.Any(z => z.Id == o.Value.Id))
            //Select the Role
            .Select(o => o.Value);

        var ToRemove = RoleMap
            //Find all roles greater then current role.
            .Where(o => o.Key > DesiredRole)
            //Filter by roles the user does have.
            .Where(o => User.Roles.Any(z => z.Id == o.Value.Id))
            //Select the Role
            .Select(o => o.Value);

        var customname = logic.Roles.GetRoleLogic(DesiredRole)?.CustomName;
        string RoleName = !string.IsNullOrWhiteSpace(customname) ? customname : DesiredRole.ToString();
        //Make a formatted message return.
        sb.AppendLine($"Setting {User.Mention} to role {RoleName}");

        foreach (var remove in ToRemove)
            sb.AppendLine($"\tRemoved Role {remove.Mention}");
        foreach (var add in ToAdd)
            sb.AppendLine($"\tAdded Role {add.Mention}");


        //Remove applicable roles
        if (ToRemove.Count() > 0)
            await User.RemoveRolesAsync(ToRemove);

        //Add applicable roles
        if (ToAdd.Count() > 0)
            await User.AddRolesAsync(ToAdd);
    }


    public enum PromotableRoleLevel
    {
        None = RoleLevel.None,
        Guest = RoleLevel.Guest,
        Member = RoleLevel.Member,
        SuperMember = RoleLevel.SuperMember,
        Officer = RoleLevel.Officer,
        Leader = RoleLevel.Leader
    }

    private RoleLevel? getActualRole(PromotableRoleLevel? role) => role is null ? null : (RoleLevel)role;
}
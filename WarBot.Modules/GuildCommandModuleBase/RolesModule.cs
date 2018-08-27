using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WarBot.Attributes;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Modules.GuildCommandModules
{
    /// <summary>
    /// This module manages a user's roles.
    /// </summary>
    //Required chat context type.
    [RequireContext(ContextType.Guild)]

    public class RolesModule : GuildCommandModuleBase
    {
        [Command("demote")]
        [Summary("Demote user[s] to the previous role."), RoleLevel(RoleLevel.Officer), RequireBotPermission(GuildPermission.ManageRoles)]
        [CommandUsage("{prefix}, demote @User @User2....")]
        public async Task DemoteUser(params SocketGuildUser[] Users)
        {
            var Me = Context.Guild.CurrentUser;
            var SendingUser = this.Context.User as SocketGuildUser;

            //Basic error handling... Make sure all of the expected values are populated.
            if (SendingUser == null) throw new System.NullReferenceException("The calling user was null");

            //This will hold the returned message.
            StringBuilder sb = new StringBuilder();

            var CallingUserCurrentRole = SendingUser.GetRole(cfg);

            var RoleMap = cfg.GetRoleMap();

            //Filter the roles to roles managable by WarBot.
            filterRoleMap(RoleMap, sb, Me);

            foreach (var TargetUser in Users)
            {
                var TargetUserCurrentRole = TargetUser.GetRole(cfg);
                var TargetUserDesiredRole = TargetUserCurrentRole.GetDemoteRole(RoleMap) ?? RoleLevel.None;

                if (TargetUserCurrentRole == RoleLevel.None)
                    sb.AppendLine($"I cannot demote {TargetUser.Mention} any further.");
                //Make sure to not manage myself.
                else if (TargetUser.Id == Me.Id)
                    sb.AppendLine("I cannot manage my own role. Only a server admin can do that.");
                //User's cannot promote to a role equal or higher then their current role.
                else if (TargetUserDesiredRole >= CallingUserCurrentRole)
                    sb.AppendLine($"You cannot demote {TargetUser.Mention}, Because the target role would be equal or higher then your current role.");
                //User cannot promote to a role equal or higher then their current role.
                else if (TargetUserCurrentRole >= CallingUserCurrentRole)
                    sb.AppendLine($"You cannot demote {TargetUser.Mention}, because they are in an equal or higher role then you.");
                else
                    await SetRole(TargetUser, RoleMap, TargetUserDesiredRole, sb);
            }

            //Send the formatted return message.
            await ReplyAsync(sb.ToString());
        }

        [Command("promote")]
        [Summary("Promote user[s] to the next available role."), RoleLevel(RoleLevel.Officer), RequireBotPermission(GuildPermission.ManageRoles)]
        [CommandUsage("{prefix}, promote @User @User1....")]
        public async Task PromoteUser(params SocketGuildUser[] Users)
        {
            var Me = Context.Guild.CurrentUser;
            var SendingUser = this.Context.User as SocketGuildUser;

            //Basic error handling... Make sure all of the expected values are populated.
            if (SendingUser == null) throw new System.NullReferenceException("The calling user was null");

            //This will hold the returned message.
            StringBuilder sb = new StringBuilder();

            var CallingUserCurrentRole = SendingUser.GetRole(cfg);

            var RoleMap = cfg.GetRoleMap();

            //Filter the roles to roles managable by WarBot.
            filterRoleMap(RoleMap, sb, Me);

            foreach (var TargetUser in Users)
            {
                var TargetUserCurrentRole = TargetUser.GetRole(cfg);
                var TargetuserDesiredRole = TargetUserCurrentRole.GetPromoteRole(RoleMap);

                if (TargetuserDesiredRole == RoleLevel.None)
                    sb.AppendLine($"There are no higher roles I can promote {TargetUser.Mention} to. I cannot promote users to a server admin role or higher.");
                //Make sure to not manage myself.
                else if (TargetUser.Id == Me.Id)
                    sb.AppendLine("I cannot manage my own role. Only a server admin can do that.");
                //If there is no higher role to promote a user to.
                else if (!TargetuserDesiredRole.HasValue)
                    sb.AppendLine($"I could not find a higher configured role for {TargetUser.Mention}");
                //User's cannot promote to a role equal or higher then their current role.
                else if (TargetuserDesiredRole >= CallingUserCurrentRole)
                    sb.AppendLine($"You promote {TargetUser.Mention}, Because the target role would be equal or higher then your current role.");
                //User cannot promote to a role equal or higher then their current role.
                else if (TargetUserCurrentRole >= CallingUserCurrentRole)
                    sb.AppendLine($"You cannot manage {TargetUser.Mention}, because they are in an equal or higher role then you.");
                else
                    await SetRole(TargetUser, RoleMap, TargetuserDesiredRole.Value, sb);
            }

            //Send the formatted return message.
            await ReplyAsync(sb.ToString());
        }

        [Command("set role"), Alias("add role", "remove role")]
        [Summary("Set user[s]'s role."), RoleLevel(RoleLevel.Officer), RequireBotPermission(GuildPermission.ManageRoles)]
        [CommandUsage("{prefix}, set role (Role) @User @User1....")]
        public async Task SetRole_User(string Role, params SocketGuildUser[] Users)
        {
            var Me = Context.Guild.CurrentUser;
            var SendingUser = this.Context.User as SocketGuildUser;

            //Basic error handling... Make sure all of the expected values are populated.
            if (SendingUser == null) throw new System.NullReferenceException("The calling user was null");

            //This will hold the returned message.
            StringBuilder sb = new StringBuilder();

            var SendingUserRole = SendingUser.GetRole(cfg);
            //ToDo - Add more error handling to the below code. There are many potential areas for it to break.
            //Parse Role into Role enum.
            if (System.Enum.TryParse(Role, true, out RoleLevel targetRole))
            {
                var RoleMap = cfg.GetRoleMap();

                //Filter roles which WarBOT cannot manage.
                filterRoleMap(RoleMap, sb, Me);

                if (targetRole >= SendingUserRole)
                    sb.AppendLine($"You cannot add or remove users from the {targetRole.ToString()} role, because it is equal or greater then your current role. You may only manage roles less then your current role.");
                else
                    foreach (var user in Users)
                        if (user.GetRole(cfg) >= SendingUserRole)
                            sb.AppendLine($"You cannot the role of {user.Mention}, because they are in an equal or higher role then you.");
                        else if (user.Id == Me.Id)
                            sb.AppendLine("I cannot manage my own roles. Only the server admin can do that.");
                        else
                            await SetRole(user, RoleMap, targetRole, sb);
            }
            else
            {
                //Was unable to parse a role level from the input text.
                sb.AppendLine("I was unable to parse the desired role from your input. The accepted values are:");
                foreach (RoleLevel val in PermissionHelper.GetRoleValues().Where(o => o < RoleLevel.ServerAdmin))
                    sb.AppendLine($"\t{val.ToString()}");

            }

            //Send the formatted return message.
            await ReplyAsync(sb.ToString());
        }

        [Command("set role")]
        [Summary("Updates the discord role mapping."), RoleLevel(RoleLevel.ServerAdmin), RequireBotPermission(GuildPermission.SendMessages)]
        [CommandUsage("{prefix}, set role (Role) @DiscordRole")]
        public async Task SetRoleInConfig(string Role = "INVALID ROLE", SocketRole RoleTag = null)
        {
            var Me = Context.Guild.CurrentUser;

            StringBuilder sb = new StringBuilder();

            if (System.Enum.TryParse(Role, true, out RoleLevel targetRole))
            {
                if (targetRole == RoleLevel.GlobalAdmin || targetRole == RoleLevel.None)
                {
                    sb.AppendLine($"You cannot manage the {targetRole.ToString()} role.");
                }
                else if (RoleTag == null)
                {
                    sb.AppendLine("Please remember to tag a role for this command. Proper Syntex:")
                        .AppendLine("bot, set role {RoleType} @TagARoleHere");
                }
                else
                {
                    #region Check to ensure WarBOT can manage all of the configured roles
                    var MyHighestRole = Me.Roles.Max(o => o.Position);

                    if (RoleTag.Position >= MyHighestRole)
                        sb.AppendLine($"Note: I will not be able to add or remove users from {RoleTag.Mention}, because this role is equal to, or above my current role. Please adjust my role position higher to compensate for this.");
                    #endregion

                    cfg.SetGuildRole(targetRole, RoleTag);

                    await cfg.SaveConfig();

                    sb.AppendLine($"Role {RoleTag.Mention} has been assigned to {targetRole.ToString()}");
                }
            }
            else
            {
                //Was unable to parse a role level from the input text.
                sb.AppendLine("I was unable to parse the desired role from your input. The accepted values are:");
                var validRoles = PermissionHelper.GetRoleValues()
                    .Where(o => o != RoleLevel.GlobalAdmin && o != RoleLevel.None)
                    .OrderBy(o => (int)o);
                foreach (RoleLevel val in validRoles)
                {
                    sb.AppendLine($"\t{val.ToString()}");
                }
            }

            //Send the formatted return message.
            await ReplyAsync(sb.ToString());
        }
        /// <summary>
        /// Remove roles from the map above the bot's highest role. Will add debug output to the string builder, to return as a message.
        /// </summary>
        /// <param name="Map"></param>
        /// <param name="sb"></param>
        /// <param name="HighestRole"></param>
        private void filterRoleMap(IDictionary<RoleLevel, IRole> Map, StringBuilder sb, SocketGuildUser WarBot)
        {
            foreach (IGrouping<ulong, KeyValuePair<RoleLevel, IRole>> r in Map.OrderByDescending(o => (int)o.Key).GroupBy(o => o.Value.Id))
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
                sb.AppendLine("I cannot add or remove users from these roles specified in my configuration, because the roles are equal to, or above my current role.");

                foreach (var x in CannotManageTheseRoles.ToArray())
                {
                    //Remove those values from the role map.
                    Map.Remove(x.Key);

                    //Append the output text.
                    if (x.Value.IsMentionable)
                        sb.AppendLine($"\tRole {x.Key.ToString()}: {x.Value.Mention}");
                    else
                        sb.AppendLine($"\tRole {x.Key.ToString()}: {x.Value.Name}");
                }
            }
        }
        private async Task SetRole(SocketGuildUser User, IDictionary<RoleLevel, IRole> RoleMap, RoleLevel DesiredRole, StringBuilder sb)
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

            //Make a formatted message return.
            sb.AppendLine($"Setting {User.Mention} to role {DesiredRole.ToString()}");

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
    }
}
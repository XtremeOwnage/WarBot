using Discord.Commands;
using System;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.ModuleType;

namespace WarBot.Attributes
{

    /// <summary>
    /// Validate a user's guild role.
    /// ONLY VALID FOR <see cref="WarBot.Core.ModuleType.GuildCommandModuleBase"/>
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleLevelAttribute : PreconditionAttribute
    {
        private RoleLevel requiredRole;
        private RoleMatchType matchType;
        public RoleLevelAttribute(RoleLevel requiredRole, RoleMatchType MatchType = RoleMatchType.GREATER_THEN_OR_EQUAL)
        {
            this.requiredRole = requiredRole;
            this.matchType = MatchType;
        }

        public bool hasPermission(RoleLevel role)
        {
            switch (matchType)
            {
                case RoleMatchType.EQUALS:
                    if (role == requiredRole)
                        return true;
                    return false;
                case RoleMatchType.GREATER_THEN:
                    if (role > requiredRole)
                        return true;
                    return false;
                case RoleMatchType.GREATER_THEN_OR_EQUAL:
                    if (role >= requiredRole)
                        return true;
                    return false;
                case RoleMatchType.LESS_THEN:
                    if (role < requiredRole)
                        return true;
                    return false;
                case RoleMatchType.LESS_THEN_OR_EQUAL:
                    if (role <= requiredRole)
                        return true;
                    return false;
                case RoleMatchType.NOT_EQUAL:
                    if (role != requiredRole)
                        return true;
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("Unknown match type encountered.");
            }

        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            //If this a custom guild command context, get the config from the context.
            if (context is GuildCommandContext gcc)
            {
                RoleLevel role = gcc.GuildUser.GetRole(gcc.cfg);

                if (hasPermission(role))
                    return PreconditionResult.FromSuccess();

                return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
            }
            else
            {
                await Task.FromResult(true);
                throw new InvalidOperationException($"{nameof(RoleLevelAttribute)} is only valid on type {nameof(GuildCommandContext)}");
            }


        }

        /// <summary>
        /// A custom type of precondition results. The logging functions utilize this to log access denied.
        /// </summary>
        public class AccessDeniedPreconditionResult : PreconditionResult
        {
            public RoleLevel UserRole;
            public RoleMatchType MatchType;
            public RoleLevel RequiredRole;
            public AccessDeniedPreconditionResult(RoleLevel UserRole, RoleMatchType Match, RoleLevel Required)
                : base(CommandError.UnmetPrecondition, "Access Denied")
            {
                this.UserRole = UserRole;
                this.MatchType = Match;
                this.RequiredRole = Required;
            }
        }
    }
}


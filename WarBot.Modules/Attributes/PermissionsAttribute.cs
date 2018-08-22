using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using WarBot.Core;
using WarBot.Core.ModuleType;
using WarBot.Modules;

namespace WarBot.Attributes
{

    public class RoleLevelAttribute : PreconditionAttribute
    {
        private RoleLevel requiredRole;
        private RoleMatchType matchType;
        public RoleLevelAttribute(RoleLevel requiredRole, RoleMatchType MatchType = RoleMatchType.GREATER_THEN_OR_EQUAL)
        {
            this.requiredRole = requiredRole;
            this.matchType = MatchType;
        }
        public async override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            IGuildConfig cfg;

            //If this a custom guild command context, get the config from the context.
            if (context is GuildCommandContext gcc)
                cfg = gcc.cfg;
            //Else, query the DI system to locate the config.
            else
                cfg = await services.GetService<IGuildConfigRepository>().GetConfig(context.Guild as SocketGuild);


            var User = context.User as SocketGuildUser;

            if (User == null || cfg == null)
                throw new NullReferenceException("Guild Config and/or User is null for RoleLevelAttribute. Please validate this attribute is only utilized on guild functions.");

            RoleLevel role = User.GetRole(cfg);

            switch (matchType)
            {
                case RoleMatchType.EQUALS:
                    if (role == requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
                case RoleMatchType.GREATER_THEN:
                    if (role > requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
                case RoleMatchType.GREATER_THEN_OR_EQUAL:
                    if (role >= requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
                case RoleMatchType.LESS_THEN:
                    if (role < requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
                case RoleMatchType.LESS_THEN_OR_EQUAL:
                    if (role <= requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
                case RoleMatchType.NOT_EQUAL:
                    if (role != requiredRole)
                        return PreconditionResult.FromSuccess();
                    return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
            }

            return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
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


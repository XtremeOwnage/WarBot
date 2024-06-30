namespace WarBot.Modules.Base;

/// <summary>
/// Validate a user's guild role.
/// ONLY VALID FOR <see cref="WarBOTModule"/>
/// </summary>
/// 
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class RoleLevelAttribute : PreconditionAttribute
{
    private RoleLevel requiredRole;
    private RoleMatchType matchType;
    public RoleLevelAttribute(RoleLevel requiredRole, RoleMatchType MatchType = RoleMatchType.GREATER_THEN_OR_EQUAL)
    {
        this.requiredRole = requiredRole;
        matchType = MatchType;
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

    public override async Task<PreconditionResult> CheckRequirementsAsync(IInteractionContext context, ICommandInfo commandInfo, IServiceProvider services)
    {
        //User is global admin.
        if (BotConfig.SUPERADMIN_USER_IDS.Contains(context.User.Id))
            return PreconditionResult.FromSuccess();
        //No Guild Context, means we cannot validate or verify permissions.
        else if (context.Guild is null)
            throw new InvalidOperationException($"No Guild Context provided.");
        //If this a custom guild command context, get the config from the context.
        else if (context is WarBOTInteractionContext wbic)
            return await wbic.UseGuildLogicAsync(async logic =>
            {
                var guildUser = context.User as IGuildUser;

                var role = guildUser.GetRole(logic);

                if (hasPermission(role))
                    return PreconditionResult.FromSuccess();

                return new AccessDeniedPreconditionResult(role, matchType, requiredRole);
            });
        else
            throw new InvalidOperationException($"{nameof(RoleLevelAttribute)} is only valid for guild commands.");
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
            : base(InteractionCommandError.UnmetPrecondition, "Access Denied")
        {
            this.UserRole = UserRole;
            MatchType = Match;
            RequiredRole = Required;
        }
    }
}

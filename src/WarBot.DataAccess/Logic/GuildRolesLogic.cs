using Discord;
using WarBot.Core;
using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic;
public class GuildRolesLogic : LogicBase<GuildRoles>
{
    internal GuildRolesLogic(GuildLogic parentLogic, GuildRoles Entity) : base(parentLogic, Entity) { }

    public RoleLogic Role_Guest => LoadRelatedLogic(o => o.Guest, o => new RoleLogic(GuildLogic, o));
    public RoleLogic Role_Member => LoadRelatedLogic(o => o.Member, o => new RoleLogic(GuildLogic, o));
    public RoleLogic Role_SuperMember => LoadRelatedLogic(o => o.SuperMember, o => new RoleLogic(GuildLogic, o));
    public RoleLogic Role_Officer => LoadRelatedLogic(o => o.Officer, o => new RoleLogic(GuildLogic, o));
    public RoleLogic Role_Leader => LoadRelatedLogic(o => o.Leader, o => new RoleLogic(GuildLogic, o));
    public RoleLogic Role_ServerAdmin => LoadRelatedLogic(o => o.ServerAdmin, o => new RoleLogic(GuildLogic, o));

    public IRole? GetRole(RoleLevel roleLevel) => GetRoleLogic(roleLevel)?.GetRole();
    public string GetRoleName(RoleLevel Role)
    {
        var logic = GetRoleLogic(Role);
        if (logic is null)
            return Role.ToString();
        if (!string.IsNullOrEmpty(logic.CustomName))
            return logic.CustomName;
        return Role.ToString();
    }
    public RoleLogic? GetRoleLogic(RoleLevel roleLevel) => roleLevel switch
    {
        RoleLevel.Guest => Role_Guest,
        RoleLevel.Member => Role_Member,
        RoleLevel.SuperMember => Role_SuperMember,
        RoleLevel.Officer => Role_Officer,
        RoleLevel.Leader => Role_Leader,
        RoleLevel.ServerAdmin => Role_ServerAdmin,
        _ => null
    };
    public void SetRole(RoleLevel roleLevel, IRole? Role) => GetRoleLogic(roleLevel)?.SetRole(Role);


    public IDictionary<RoleLevel, RoleLogic?> GetRoleMap()
    {
        return Enum.GetValues<RoleLevel>()
            .OrderBy(o => (int)o)
            .ToDictionary(o => o, o => GetRoleLogic(o));
    }

    //Clear and reset all configuration.
    public override void ClearSettings()
    {
        foreach (var r in GetRoleMap().Values)
            r?.ClearSettings();
    }

}

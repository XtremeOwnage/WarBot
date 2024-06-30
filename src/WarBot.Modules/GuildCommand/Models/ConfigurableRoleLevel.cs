namespace WarBot.Modules.GuildCommand.Models;
public enum ConfigurableRoleLevel
{
    None = RoleLevel.None,
    Guest = RoleLevel.Guest,
    Member = RoleLevel.Member,
    SuperMember = RoleLevel.SuperMember,
    Officer = RoleLevel.Officer,
    Leader = RoleLevel.Leader
}

public static class ConfigurableRoleLevelExtensions
{
    public static RoleLevel ToRoleLevel(this ConfigurableRoleLevel role) => (RoleLevel)role;
}
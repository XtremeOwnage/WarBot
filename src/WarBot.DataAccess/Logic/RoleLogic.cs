using Discord;
using WarBot.DataAccess.Logic.Base;

namespace WarBot.DataAccess.Logic;
public class RoleLogic : LogicBase<GuildRole>
{
    internal RoleLogic(GuildLogic parentLogic, GuildRole Entity) : base(parentLogic, Entity) { }

    public IRole? GetRole()
    {
        if (entity.DiscordID is null)
            return null;

        return guild.GetRole(entity.DiscordID.Value);
    }
    public void SetRole(IRole? Role)
    {
        if (entity is null)
            entity = new GuildRole
            {
                DiscordID = Role?.Id,
                DiscordName = Role?.Name,
            };
        else
        {
            //Update the existing Object.
            entity.DiscordID = Role?.Id;
            entity.DiscordName = Role?.Name;
        }
    }

    public string? CustomName
    {
        get => entity.CustomName;
        set => entity.CustomName = value;
    }

    public ulong? RoleID => entity.DiscordID;
    public string? RoleName => entity.DiscordName;

    public override void ClearSettings()
    {
        this.SetRole(null);
        this.CustomName = null;
    }
}

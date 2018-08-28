using System.ComponentModel;

namespace WarBot.Core
{
    public enum RoleLevel
    {

        [Description("Regular user in discord guild, without any clan-assigned roles")]
        None = 0,

        [Description("A role for guests. Does not grant member-specific commands.")]
        Guest = 5,

        [Description("A member of your guild.")]
        Member = 10,

        [Description("A distinguished, non-officer member")]
        SuperMember = 20,

        [Description("An officer. Grants basic management commands")]
        Officer = 30,

        [Description("A leader of your guild. Grants most management commands.")]
        Leader = 50,

        [Description("Server Admin, Grants all management commands")]
        ServerAdmin = 100,

        [Description("Secret role. Grants global bot-management commands.")]
        GlobalAdmin = 1000,
    }
}

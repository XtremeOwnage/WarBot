namespace WarBot.Core
{
    public enum RoleLevel
    {
        /// <summary>
        /// Regular user in discord guild, without any clan-assigned roles.
        /// </summary>
        None = 0,

        /// <summary>
        /// A guest role.
        /// </summary>
        Guest = 5,

        /// <summary>
        /// User is in the member's role.
        /// </summary>
        Member = 10,

        /// <summary>
        /// A distinguished member.
        /// </summary>
        SuperMember = 20,

        /// <summary>
        /// User is in the Officer's role
        /// </summary>
        Officer = 30,

        /// <summary>
        /// User is in the leader's role
        /// </summary>
        Leader = 50,

        /// <summary>
        /// User contains the administrator permission for the server.
        /// </summary>
        ServerAdmin = 100,

        /// <summary>
        /// This role, is for statically defined administrators of the bot. Grants the ability to manage GLOBAL settings for WarBOT.
        /// </summary>
        GlobalAdmin = 1000,
    }
}

namespace WarBot.Core
{
    public enum RoleLevel
    {
        /// <summary>
        /// Regular user in discord guild, without any clan-assigned roles.
        /// </summary>
        None = 0,

        /// <summary>
        /// User is in the member's role.
        /// </summary>
        Member = 1,

        /// <summary>
        /// User is in the Officer's role
        /// </summary>
        Officer = 2,

        /// <summary>
        /// User is in the leader's role
        /// </summary>
        Leader = 3,

        /// <summary>
        /// User contains the administrator permission for the server.
        /// </summary>
        ServerAdmin = 5,

        /// <summary>
        /// This role, is for statically defined administrators of the bot. Grants the ability to manage GLOBAL settings for WarBOT.
        /// </summary>
        GlobalAdmin = 10,
    }
}

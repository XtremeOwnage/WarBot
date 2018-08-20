using Discord;

namespace WarBot.Core
{
    public class PermissionHelper
    {
        public static bool TestPermission(IGuildChannel Channel, ChannelPermission Permission, IGuildUser User)
        => User.GetPermissions(Channel).ToList().Contains(Permission);

        public static bool TestPermission(IGuildChannel Channel, ChannelPermission Permission, IRole Role)
            => Channel.GetPermissionOverwrite(Role)?.ToAllowList().Contains(Permission) ?? false;

    }
}

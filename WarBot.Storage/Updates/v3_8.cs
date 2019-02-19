using WarBot.Core;
using WarBot.Core.Updates;

namespace WarBot.Storage.Updates
{
    public class v3_8 : IBotUpdate
    {
        public double Version => 3.8;

        public string ReleaseNotesURL => "https://github.com/XtremeOwnage/WarBot/blob/master/ChangeLogs/v3.8.md";

        public bool SendUpdateToGuild => true;

        public void Apply(IGuildConfig cfg)
        {
            //3.7 added a new portal channel. For guilds we update- make sure to set the portal channel, to the old war channel.
            cfg.SetGuildChannel(WarBotChannelType.PORTAL, cfg.GetGuildChannel(WarBotChannelType.WAR));

            //The new user notifications do not include the mention by default. Lets mimmick the setting.
            var uj = cfg[Setting_Key.USER_JOIN];
            if (uj.Enabled && uj.HasValue)
                uj.Value = "{user}, " + uj.Value;

    }
    }
}

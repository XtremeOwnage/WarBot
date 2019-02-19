using WarBot.Core;
using WarBot.Core.Updates;

namespace WarBot.Storage.Updates
{
    public class v3_7 : IBotUpdate
    {
        public double Version => 3.7;

        public string ReleaseNotesURL => "https://github.com/XtremeOwnage/WarBot/blob/master/ChangeLogs/v3.7.md";

        public bool SendUpdateToGuild => true;

        public void Apply(IGuildConfig config)
        {
            //No changes.
        }
    }
}

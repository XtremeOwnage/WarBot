using Discord.Commands;
using WarBot.Core;

namespace WarBot.Modules
{
    public abstract class GuildCommandModuleBase : ModuleBase<GuildCommandContext>
    {
        public IGuildConfig cfg => this.Context.cfg;
    }
}

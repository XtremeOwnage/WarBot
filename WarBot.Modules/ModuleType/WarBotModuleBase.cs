using Discord.Commands;
using WarBot.Core;

namespace WarBot.Modules
{
    public abstract class WarBotModuleBase : ModuleBase<GuildCommandContext>
    {
        public IGuildConfig cfg => this.Context.cfg;
    }
}

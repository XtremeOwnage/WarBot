using Discord.Commands;
using WarBot.Core;

namespace WarBot.Modules
{
    public abstract class GuildCommandModuleBase : ModuleBase<GuildCommandContext>
    {
        ///Just a helper.
        public IGuildConfig cfg => this.Context.cfg;

        //Just another helper.
        public IWARBOT bot => this.Context.bot;
    }
}

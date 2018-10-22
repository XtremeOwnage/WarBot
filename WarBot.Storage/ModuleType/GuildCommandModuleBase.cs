using Discord.Commands;
using WarBot.Core;

namespace WarBot.Core.ModuleType
{
    [RequireContext(ContextType.Guild)]
    public abstract class GuildCommandModuleBase : ModuleBase<GuildCommandContext>
    {
        ///Just a helper.
        public IGuildConfig cfg => this.Context.cfg;

        //Just another helper.
        public IWARBOT bot => this.Context.bot;

        public RoleLevel UserRole => this.Context.GuildUser.GetRole(cfg);
    }
}


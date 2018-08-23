using Discord.Commands;

namespace WarBot.Core.ModuleType
{
    /// <summary>
    /// Use as the default Modulebase type. Contains a reference to IWarBOT.
    /// </summary>
    public abstract class CommandModuleBase : ModuleBase<CommandContext>
    {
        //Just another helper.
        public IWARBOT bot => this.Context.bot;
    }
}

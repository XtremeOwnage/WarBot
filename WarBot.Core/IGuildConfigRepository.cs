using System.Threading.Tasks;
using Discord;

namespace WarBot.Core
{
    public interface IGuildConfigRepository
    {
        Task<IGuildConfig> GetConfig(IGuild Guild);
    }
}
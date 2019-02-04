using System.Threading.Tasks;
using Discord.WebSocket;

namespace WarBot.Core
{
    public interface ITaskBOT
    {
        Task ClearMessages(SocketTextChannel Channel, bool DeletePinned = false);
    }
}
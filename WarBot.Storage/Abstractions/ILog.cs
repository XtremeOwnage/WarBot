using Discord;
using Discord.Commands;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WarBot.Core
{
    public interface ILog
    {
        Task ChatMessage(IMessage Message, IGuild Guild, IResult Result);
        Task ConsoleOUT(string Message);
        Task Debug(string Message, IGuild Guild = null);
        Task Error(IGuild guild, Exception ex, [CallerMemberName] string Method = "");
        bool IsLoggingChannel(ulong CHID);
    }
}
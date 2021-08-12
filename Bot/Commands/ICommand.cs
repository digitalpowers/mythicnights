using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MythicNights
{
    public interface ICommand
    {
        string Command { get; }
        Task<Response> CommandReceived(string[] parameters, IUser user, SocketUserMessage message);
        string Help();
    }
}

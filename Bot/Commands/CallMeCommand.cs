using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MythicNights
{
    internal class CallMeCommand : ICommand
    {
        private readonly PlayerManager playerManager;

        public CallMeCommand(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }
        public string Command => "!callme";

        public async Task<Response> CommandReceived(string[] parameters, IUser user, SocketUserMessage message)
        {
            var nickname = string.Join(" ", parameters);
            if(string.IsNullOrEmpty(nickname))
            {
                return Response.Failulre("expected a nickname to call you by");
            }

            var response = await playerManager.SetNick(user.Id, user.Username, nickname);
            return response;
        }

        public string Help()
        {
            return "expects a new nickname to call you. example: !callme bob";
        }
    }
}

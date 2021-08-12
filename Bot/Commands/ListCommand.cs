using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace MythicNights
{
    internal class ListCommand : ICommand
    {
        private PlayerManager playerManager;

        public ListCommand(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }
        public string Command => "!list";

        public async Task<Response> CommandReceived(string[] parameters, IUser user, SocketUserMessage message)
        {
            if (parameters.Length != 0)
            {
                return Response.Failulre($"Expected only 0 parameter, received {parameters.Length}");
            }

            var player = await playerManager.GetPlayer(user.Id, user.Username);
            string toons = "";
            foreach(var t in player.Toons)
            {
                var toonString = $"{t.FullName} RIO: {t.RaiderIO} Ilvl: {t.iLvl} Main: '{t.PreferedRole}' Offspec: '{t.Offspec}'";
                if(t.IsPrefered.HasValue)
                {
                    if(t.IsPrefered.Value)
                    {
                        toonString += " Prefered Toon";
                    }
                    else
                    {
                        toonString += " Unprefered Toon";
                    }
                }
                toons += $"{toonString}\n";
            }
            string response = $@"```
{player.Nickname}
================================================
{toons}
```"; 

            return Response.Success(response);
        }

        public string Help()
        {
            return "lists the toons you have registered";
        }
    }
}

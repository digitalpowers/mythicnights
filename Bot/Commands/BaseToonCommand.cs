using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using MythicNights.DataContext;

namespace MythicNights
{
    internal abstract class BaseToonCommand : ICommand
    {
        protected readonly PlayerManager playerManager;

        public BaseToonCommand(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }
        public virtual string Command => throw new System.NotImplementedException();

        public async Task<Response> CommandReceived(string[] parameters, IUser user, SocketUserMessage message)
        {
            if (parameters.Length < ExpectedParameters)
            {
                return Response.Failulre($"Expected {ExpectedParameters} received {parameters.Length}");
            }
            var toonId = parameters[0];
            var nameRegex1 = @"\/?(?<reg>[^\/]*)\/(?<realm>[^\/]*)\/(?<name>[^\/]*)";
            var nameRegex2 = @"^(?<name>[^\-]*)-(?<realm>.*)$";

            var match = Regex.Match(toonId, nameRegex1);
            if (!match.Success)
            {
                match = Regex.Match(toonId, nameRegex2);
            }
            if (!match.Success)
            {
                return Response.Failulre($"could not parse {toonId} into either region/realm/name or name-realm format (ex: kalefupanda-eonar)");
            }

            var player = await playerManager.GetPlayer(user.Id, user.Username);
            return await CommandReceived(player, match.Groups["name"].Value, match.Groups["realm"].Value, parameters.Skip(1).ToArray());
        }

        protected virtual int ExpectedParameters => 1;

        public abstract Task<Response> CommandReceived(Player player, string name, string realm, string[] parameters);

        public virtual string Help()
        {
            return "expectes a character in the format name-realm or region/realm/name (examples: kaleminos-eonar us/eonar/kaleminos)";
        }
    }

    //TODO: add admin commands
    // - base admin tool that uses role on server
    // - list all attending players etc
    // - list all players
    // - get group options

}

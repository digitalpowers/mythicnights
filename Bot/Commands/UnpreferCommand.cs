using System.Threading.Tasks;
using MythicNights.DataContext;

namespace MythicNights
{
    internal class UnpreferCommand : BaseToonCommand
    {
        public UnpreferCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!unprefer";

        public override async Task<Response> CommandReceived(Player player, string name, string realm, string[] parameters)
        {
            var response = await playerManager.SetToonPreference(player, name, realm, false);
            return response;
        }
    }
}

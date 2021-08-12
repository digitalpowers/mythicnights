using System.Threading.Tasks;
using MythicNights.DataContext;

namespace MythicNights
{
    internal class PreferCommand : BaseToonCommand
    {
        public PreferCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!prefer";

        public override async Task<Response> CommandReceived(Player player, string name, string realm, string[] parameters)
        {
            var response = await playerManager.SetToonPreference(player, name, realm, true);
            return response;
        }
    }


}

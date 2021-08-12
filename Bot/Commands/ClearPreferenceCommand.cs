using System.Threading.Tasks;
using MythicNights.DataContext;

namespace MythicNights
{
    internal class ClearPreferenceCommand : BaseToonCommand
    {
        public ClearPreferenceCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!clearpreference";

        public override async Task<Response> CommandReceived(Player player, string name, string realm, string[] parameters)
        {
            var response = await playerManager.SetToonPreference(player, name, realm, null);
            return response;
        }
    }


}

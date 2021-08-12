using System.Threading.Tasks;

namespace MythicNights
{
    internal class AddToonCommand : BaseToonCommand
    {
        public AddToonCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!add";

        public override async Task<Response> CommandReceived(DataContext.Player player, string name, string realm, string[] parameters)
        {
            var response = await playerManager.AddToon(player, name, realm);
            return response;
        }
    }
}

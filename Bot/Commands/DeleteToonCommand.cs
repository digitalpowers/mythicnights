using System.Threading.Tasks;

namespace MythicNights
{
    internal class DeleteToonCommand : BaseToonCommand
    {
        public DeleteToonCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!delete";

        public override async Task<Response> CommandReceived(DataContext.Player player, string name, string realm, string[] parameters)
        {
            var response = await playerManager.DeleteToon(player, name, realm);
            return response;
        }
    }
}

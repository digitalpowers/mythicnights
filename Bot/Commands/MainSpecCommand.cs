using System;
using System.Threading.Tasks;
using MythicNights.DataContext;

namespace MythicNights
{
    internal class MainSpecCommand : BaseToonCommand
    {
        public MainSpecCommand(PlayerManager playerManager) : base(playerManager)
        {
        }
        override public string Command => "!mainspec";
        protected override int ExpectedParameters => 2;
        public override async Task<Response> CommandReceived(Player player, string name, string realm, string[] parameters)
        {
            if (Enum.TryParse<Role>(parameters[0], true, out Role role))
            {
                return await playerManager.SetMainSpec(player, name, realm, role);
            }
            return Response.Failulre($"Couldnt parse role from {parameters[0]} expected dps, tank, healer or none");
        }
    }
}

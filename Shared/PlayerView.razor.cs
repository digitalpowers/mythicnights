using Microsoft.AspNetCore.Components;
using MythicNights.DataContext;
using System.Threading.Tasks;

namespace MythicNights.Shared
{
    public partial class PlayerView : ComponentBase
    {
        [Parameter]
        public ulong playerId { get; set; }

        [Inject]
        PlayerManager playerManager { get; set; }

        public Player player { get; set; }

        protected override async Task OnInitializedAsync()
        {
            player = await playerManager.GetPlayer(playerId);
        }

    }
}

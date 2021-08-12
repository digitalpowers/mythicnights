using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using MythicNights.Data;
using MythicNights.DataContext;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MythicNights.Pages
{
    [Authorize]
    public partial class ToonsView : ComponentBase
    {
        const string nameIdentifierType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
        [Inject]
        PlayerManager playerManager { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        UserService userService { get; set; }

        private List<Toon> Toons = new List<Toon>();

        Player player = null;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var discUserClaim = userService.GetInfo(authState);
            if (discUserClaim != null)
            {
                player = await playerManager.GetPlayer(discUserClaim.UserId, discUserClaim.Name);
                Toons = player.Toons;
            }
        }

        public async Task Update()
        {
            if (player != null)
            {
                await playerManager.UpdateToons(player);
            }
        }
    }
}

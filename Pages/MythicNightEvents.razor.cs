using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
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
    public partial class MythicNightEvents : ComponentBase
    {
        [Inject]
        EventManager eventManager { get; set; }

        [Inject]
        IHttpContextAccessor httpContextAccessor { get; set; }

        [Inject]
        UserService userService { get; set; }

        private List<MythicNight> Events;

        private List<Group> groups = new List<Group>();

        protected override async Task OnInitializedAsync()
        {
            //var context = httpContextAccessor.HttpContext;
            //var discUserClaim = userService.GetInfo(context);
            Events = await eventManager.GetEvents();
        }
    }
}

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
    public partial class MythicNightEvent : ComponentBase
    {
        [Parameter]
        public string eventId { get; set; }

        [Inject]
        EventManager eventManager { get; set; }

        [Inject]
        IHttpContextAccessor httpContextAccessor { get; set; }

        [Inject]
        UserService userService { get; set; }

        private MythicNight mythicNight;
        private List<ulong> ignored = new List<ulong>();

        private List<Group> groups = new List<Group>();

        protected override async Task OnInitializedAsync()
        {
            var id = ulong.Parse(eventId);
            mythicNight = await eventManager.GetEvent(id);
        }

        protected async Task GenerateGroups()
        {
            groups = await eventManager.CreateGroups(mythicNight.Attending);
        }

    }
}

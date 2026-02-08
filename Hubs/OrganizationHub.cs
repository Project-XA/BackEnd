using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_X.Hubs
{
    [Authorize]
    public class OrganizationHub : Hub
    {
        public async Task JoinOrganizationGroup(string organizationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Org_{organizationId}");
        }

        public async Task LeaveOrganizationGroup(string organizationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Org_{organizationId}");
        }
    }
}

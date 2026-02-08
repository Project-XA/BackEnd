using Project_X.Models.Response;
using System.Threading.Tasks;

namespace Project_X.Services
{
    public interface IOrganizationEventService
    {
        Task LogEventAsync(int organizationId, string userId, string eventType, string description);
        Task<ApiResponse> GetRecentEventsAsync(int organizationId, int count = 20);
    }
}

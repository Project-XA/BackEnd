using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.Response;
using Project_X.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Project_X.Hubs;

namespace Project_X.Services
{
    public class OrganizationEventService : IOrganizationEventService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<OrganizationHub> _hubContext;

        public OrganizationEventService(IUnitOfWork unitOfWork, Microsoft.AspNetCore.SignalR.IHubContext<OrganizationHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task LogEventAsync(int organizationId, string userId, string eventType, string description)
        {
            try
            {
                var orgEvent = new OrganizationEvent
                {
                    OrganizationId = organizationId,
                    UserId = userId,
                    EventType = eventType,
                    Description = description,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.OrganizationEvents.AddAsync(orgEvent);
                await _unitOfWork.SaveAsync();
                await _hubContext.Clients.Group($"Org_{organizationId}").SendAsync("ReceiveEvent", new 
                {
                    Id = orgEvent.Id,
                    OrganizationId = organizationId,
                    UserId = userId,
                    EventType = eventType,
                    Description = description,
                    CreatedAt = orgEvent.CreatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging event: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetRecentEventsAsync(int organizationId, int count = 20)
        {
            var events = await _unitOfWork.OrganizationEvents.FindAllAsync(e => e.OrganizationId == organizationId, new[] { "User" });
            var recentEvents = events.OrderByDescending(e => e.CreatedAt).Take(count).ToList();

            return ApiResponse.SuccessResponse("Recent events retrieved successfully", recentEvents);
        }
    }
}

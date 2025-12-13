using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface ISessionService
    {
        Task<ApiResponse> CreateSessionAsync(CreateSessionDTO createSessionDTO, string userId);
    }
}

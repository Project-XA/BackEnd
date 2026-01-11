using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface ISessionService
    {
        Task<ApiResponse> CreateSessionAsync(CreateSessionDTO createSessionDTO, string userId);
        Task<ApiResponse> GetSessionByIdAsync(int sessionId);
        Task<ApiResponse> GetSessionsByHallIdAsync(int hallId);
        Task<ApiResponse> UpdateSessionAsync(int sessionId, UpdateSessionDTO updateSessionDTO);
        Task<ApiResponse> DeleteSessionAsync(int sessionId);
    }
}

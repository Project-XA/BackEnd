using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IHallService
    {
        Task<ApiResponse> CreateHallAsync(HallDTO hallDTO);
        Task<ApiResponse> GetAllHallsAsync(int organizationId);
        Task<ApiResponse> GetHallByIdAsync(int hallId);
        Task<ApiResponse> UpdateHallAsync(int hallId, UpdateHallDTO updateHallDTO);
        Task<ApiResponse> DeleteHallAsync(int hallId);
    }
}

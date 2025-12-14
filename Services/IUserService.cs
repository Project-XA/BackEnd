using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IUserService
    {
        Task<ApiResponse> GetUserAsync(GetUserDTO UserDTO);
        Task<ApiResponse> GetUserRoleAsync(GetUserDTO UserDTO);
    }
}

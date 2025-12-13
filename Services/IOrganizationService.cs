using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IOrganizationService
    {
        Task<ApiResponse> CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDTO, string userId);
        Task<ApiResponse> AddMemberAsync(AddMemberDTO addMemberDTO);
    }
}

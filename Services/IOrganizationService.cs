using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IOrganizationService
    {
        Task<ApiResponse> CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDTO, string userId);
        Task<ApiResponse> AddMemberAsync(AddMemberDTO addMemberDTO);
        Task<ApiResponse> GetOrganizationByIdAsync(int organizationId);
        Task<ApiResponse> UpdateOrganizationAsync(int organizationId, UpdateOrganizationDTO updateOrganizationDTO);
        Task<ApiResponse> DeleteOrganizationAsync(int organizationId);
    }
}

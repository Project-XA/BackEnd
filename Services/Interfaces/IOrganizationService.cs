using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface IOrganizationService
    {
        Task<ApiResponse> CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDTO, string userId);
        Task<ApiResponse> AddMemberAsync(AddMemberDTO addMemberDTO, string userId);
        Task<ApiResponse> GetOrganizationByIdAsync(int organizationId, string userId);
        Task<ApiResponse> UpdateOrganizationAsync(int organizationId, UpdateOrganizationDTO updateOrganizationDTO, string userId);
        Task<ApiResponse> DeleteOrganizationAsync(int organizationId, string userId);
        Task<ApiResponse> GetUserOrganizationsAsync(string userId);
        Task<ApiResponse> GetOrganizationUsersAsync(int organizationId, string userId);
        Task<ApiResponse> GenerateApiKeyAsync(int organizationId, string userId);
        Task<ApiResponse> GetOrganizationByApiKeyAsync(string apiKey);
    }
}

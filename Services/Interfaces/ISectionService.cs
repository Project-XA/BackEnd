using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public interface ISectionService
    {
        Task<ApiResponse> CreateSectionAsync(CreateSectionDTO dto, string userId);
        Task<ApiResponse> GetSectionsByOrganizationAsync(int organizationId, string userId);
        Task<ApiResponse> GetSectionByIdAsync(int sectionId, string userId);
        Task<ApiResponse> UpdateSectionAsync(int sectionId, UpdateSectionDTO dto, string userId);
        Task<ApiResponse> DeleteSectionAsync(int sectionId, string userId);
        Task<ApiResponse> AddStudentToSectionAsync(AddStudentToSectionDTO dto, string userId);
        Task<ApiResponse> RemoveStudentFromSectionAsync(int sectionId, string studentUserId, string userId);
        Task<ApiResponse> GetSectionMembersAsync(int sectionId, string userId);
        Task<ApiResponse> JoinSectionByCodeAsync(JoinSectionDTO dto, string userId);
    }
}

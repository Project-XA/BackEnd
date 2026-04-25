using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public class SectionService : ISectionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public SectionService(IUnitOfWork unitOfWork, UserManager<AppUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateSectionAsync(CreateSectionDTO dto, string userId)
        {
            var organization = await _unitOfWork.Organizations.GetByIdAsync(dto.OrganizationId);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization ID" });
            }

            if (!organization.IsUniversity)
            {
                return ApiResponse.FailureResponse("Sections can only be created for university organizations.", new List<string> { "Organization is not a university." });
            }

            var isMember = await _unitOfWork.Organizations.ValidateUser(dto.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.SuperAdmin))
            {
                return ApiResponse.FailureResponse("Unauthorized.", new List<string> { "Only admins can create sections." });
            }

            var section = _mapper.Map<Section>(dto);

            try
            {
                await _unitOfWork.Sections.AddAsync(section);
                await _unitOfWork.SaveAsync();

                var response = _mapper.Map<SectionResponseDTO>(section);

                return ApiResponse.SuccessResponse("Section created successfully.", response);
            }
            catch (Exception ex)
            {
                return ApiResponse.FailureResponse("An error occurred while creating section.", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse> GetSectionsByOrganizationAsync(int organizationId, string userId)
        {
            var isMember = await _unitOfWork.Organizations.ValidateUser(organizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization ID" });
            }

            if (!organization.IsUniversity)
            {
                return ApiResponse.FailureResponse("Sections are only available for university organizations.");
            }

            var sections = await _unitOfWork.Sections.GetSectionsByOrganizationAsync(organizationId);
            var response = _mapper.Map<List<SectionResponseDTO>>(sections);

            return ApiResponse.SuccessResponse("Sections retrieved successfully.", response);
        }

        public async Task<ApiResponse> GetSectionByIdAsync(int sectionId, string userId)
        {
            var section = await _unitOfWork.Sections.GetSectionWithUsersAsync(sectionId);
            if (section == null)
            {
                return ApiResponse.FailureResponse("Section not found.", new List<string> { "Invalid Section ID" });
            }

            var isMember = await _unitOfWork.Organizations.ValidateUser(section.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var response = _mapper.Map<SectionResponseDTO>(section);

            return ApiResponse.SuccessResponse("Section retrieved successfully.", response);
        }

        public async Task<ApiResponse> UpdateSectionAsync(int sectionId, UpdateSectionDTO dto, string userId)
        {
            var section = await _unitOfWork.Sections.GetByIdAsync(sectionId);
            if (section == null)
            {
                return ApiResponse.FailureResponse("Section not found.", new List<string> { "Invalid Section ID" });
            }

            var isMember = await _unitOfWork.Organizations.ValidateUser(section.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.SuperAdmin))
            {
                return ApiResponse.FailureResponse("Unauthorized.", new List<string> { "Only admins can update sections." });
            }

            section.SectionName = dto.SectionName;
            section.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Sections.Update(section);
            await _unitOfWork.SaveAsync();

            var response = _mapper.Map<SectionResponseDTO>(section);

            return ApiResponse.SuccessResponse("Section updated successfully.", response);
        }

        public async Task<ApiResponse> DeleteSectionAsync(int sectionId, string userId)
        {
            var section = await _unitOfWork.Sections.GetByIdAsync(sectionId);
            if (section == null)
            {
                return ApiResponse.FailureResponse("Section not found.", new List<string> { "Invalid Section ID" });
            }

            var isMember = await _unitOfWork.Organizations.ValidateUser(section.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || (user.Role != UserRole.Admin && user.Role != UserRole.SuperAdmin))
            {
                return ApiResponse.FailureResponse("Unauthorized.", new List<string> { "Only admins can delete sections." });
            }

            _unitOfWork.Sections.Delete(section);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Section deleted successfully.");
        }
    }
}

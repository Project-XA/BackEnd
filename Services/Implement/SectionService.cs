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
        private readonly Random _random = new Random();

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
            int code;
            bool isUnique = false;
            int maxRetries = 10;
            int retries = 0;

            do
            {
                code = _random.Next(1000, 9999);
                var exists = await _unitOfWork.Sections.VerifySectionCodeAsync(code);
                if (!exists)
                {
                    isUnique = true;
                }
                retries++;
            } while (!isUnique && retries < maxRetries);

            if (!isUnique)
            {
                return ApiResponse.FailureResponse("Failed to generate unique section code. Please try again.", new List<string> { "Unique code generation failed after max retries" });
            }

            var section = _mapper.Map<Section>(dto);
            section.SectionCode = code;

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

        public async Task<ApiResponse> AddStudentToSectionAsync(AddStudentToSectionDTO dto, string userId)
        {
            var section = await _unitOfWork.Sections.GetSectionWithUsersAsync(dto.SectionId);
            if (section == null)
            {
                return ApiResponse.FailureResponse("Section not found.", new List<string> { "Invalid Section ID" });
            }

            var isMember = await _unitOfWork.Organizations.ValidateUser(section.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }

            var adminUser = await _userManager.FindByIdAsync(userId);
            if (adminUser == null || (adminUser.Role != UserRole.Admin && adminUser.Role != UserRole.SuperAdmin))
            {
                return ApiResponse.FailureResponse("Unauthorized.", new List<string> { "Only admins can add students to sections." });
            }

            var student = await _userManager.FindByIdAsync(dto.UserId);
            if (student == null)
            {
                return ApiResponse.FailureResponse("Student not found.", new List<string> { "Invalid User ID" });
            }

            var alreadyInSection = section.SectionUsers?.Any(su => su.UserId == dto.UserId) ?? false;
            if (alreadyInSection)
            {
                return ApiResponse.FailureResponse("Student is already a member of this section.", new List<string> { "Duplicate membership" });
            }

            var sectionUser = new SectionUser
            {
                SectionId = dto.SectionId,
                UserId = dto.UserId,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.SectionUsers.AddAsync(sectionUser);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Student added to section successfully.",
                new { dto.SectionId, dto.UserId, student.FullName });
        }

        public async Task<ApiResponse> RemoveStudentFromSectionAsync(int sectionId, string studentUserId, string userId)
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

            var adminUser = await _userManager.FindByIdAsync(userId);
            if (adminUser == null || (adminUser.Role != UserRole.Admin && adminUser.Role != UserRole.SuperAdmin))
            {
                return ApiResponse.FailureResponse("Unauthorized.", new List<string> { "Only admins can remove students from sections." });
            }

            var sectionUser = section.SectionUsers?.FirstOrDefault(su => su.UserId == studentUserId);
            if (sectionUser == null)
            {
                return ApiResponse.FailureResponse("Student is not a member of this section.", new List<string> { "User not found in section" });
            }

            _unitOfWork.SectionUsers.Delete(sectionUser);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Student removed from section successfully.");
        }

        public async Task<ApiResponse> GetSectionMembersAsync(int sectionId, string userId)
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

            var members = _mapper.Map<List<SectionMemberResponseDTO>>(section.SectionUsers ?? new List<SectionUser>());

            return ApiResponse.SuccessResponse("Section members retrieved successfully.", members);
        }

        public async Task<ApiResponse> JoinSectionByCodeAsync(JoinSectionDTO dto, string userId)
        {
            var section = await _unitOfWork.Sections.GetByCodeAsync(dto.SectionCode);
            if (section == null)
            {
                return ApiResponse.FailureResponse("Invalid section code.", new List<string> { "No section found with this code." });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ApiResponse.FailureResponse("User not found.", new List<string> { "Invalid User ID" });
            }

            var existingMembership = await _unitOfWork.SectionUsers
                .FindAsync(su => su.SectionId == section.SectionId && su.UserId == userId);
            if (existingMembership.Any())
            {
                return ApiResponse.FailureResponse("You are already a member of this section.", new List<string> { "Duplicate membership" });
            }

            var sectionUser = new SectionUser
            {
                SectionId = section.SectionId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            await _unitOfWork.SectionUsers.AddAsync(sectionUser);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Joined section successfully.",
                new { section.SectionId, section.SectionName, section.OrganizationId });
        }
    }
}

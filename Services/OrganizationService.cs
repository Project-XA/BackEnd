using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Enums;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;
using Project_X.Models.Response;
using System.Security.Claims;

namespace Project_X.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Random _random = new Random();

        public OrganizationService(
            UserManager<AppUser> userManager,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateOrganizationAsync(CreateOrganizationDTO createOrganizationDTO, string userId)
        {
            if (userId != null)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    if (user.Role == UserRole.Admin)
                    {
                          // Begin Transaction
            // Note: In typical EF Core with UnitOfWork, if we want to ensure atomicity across multiple repository operations,
            // we rely on SaveAsync() being a single transaction.
            // However, here we are mixing UserManager (which has its own context/save) and UnitOfWork.
            // Ideally, they should share the context. 
            // Assuming they might not, we should proceed carefully. 
            // Since we are only reading from UserManager here, and writing to UnitOfWork, 
            // we effectively only have one write transaction (UnitOfWork.SaveAsync).
            // So we can do all logic and then SaveAsync once.

            var organization = _mapper.Map<Organization>(createOrganizationDTO);
            organization.CreatedAt = DateTime.UtcNow;
            int code;
            bool isUnique = false;
            int maxRetries = 10;
            int retries = 0;

            do
            {
                code = _random.Next(1000, 9999);
                var existingOrg = await _unitOfWork.Organizations.GetByCodeAsync(code);
                if (existingOrg == null)
                {
                    isUnique = true;
                }
                retries++;
            } while (!isUnique && retries < maxRetries);

            if (!isUnique)
            {
                return ApiResponse.FailureResponse("Failed to generate unique organization code. Please try again.");
            }

            organization.OrganizationCode = code;

            try
            {
                await _unitOfWork.Organizations.AddAsync(organization);
                // We must save here to get the OrganizationId if it's identity-generated, 
                // OR we can add the OrganizationUser to the context and EF might sort it out if navigation properties are set.
                // However, OrganizationUser requires OrganizationId.
                // If we set navigation property `Organization` on `OrganizationUser`, EF Core is smart enough to insert Org first, get ID, then insert User.
                
                var organizationUser = new OrganizationUser
                {
                    Organization = organization, // Use navigation property to ensure single transaction commit handles IDs
                    UserId = user.Id,
                    Role = user.Role
                };

                await _unitOfWork.OrganizationUsers.AddAsync(organizationUser);
                await _unitOfWork.SaveAsync();

                var organizationResponse = _mapper.Map<OrganizationResponseDTO>(organization);
                return ApiResponse.SuccessResponse("Organization Created Successfully", organizationResponse);
            }
            catch (Exception ex)
            {
                return ApiResponse.FailureResponse($"An error occurred: {ex.Message}");
            }
                    }
            
                }
            }
           return ApiResponse.FailureResponse("Unauthorized");
        }

        public async Task<ApiResponse> AddMemberAsync(AddMemberDTO addMemberDTO)
        {
            var existingUser = await _userManager.FindByEmailAsync(addMemberDTO.Email);
            if (existingUser != null)
            {
                return ApiResponse.FailureResponse("User with this email already exists.");
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(addMemberDTO.OrganizationId);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization Id" });
            }

            // We need to create the user AND add them to organization.
            // UserManager saves changes immediately. 
            // This is a distributed transaction problem if we don't share transaction.
            // For now, we follow the best effort: Create User first (revert if needed? Hard with Identity).
            // Or better: Use a transaction scope if checking for 'fatal mistakes'.
            
            // Strategy: Create User. If fail, return. If success, try add to Org. If that fails, delete user? 
            // Or just accept that User exists but not in Org (dangling user).
            // Given "fetal mistake" prompt, I should try to make it transactional.
            // But I cannot easily wrap UserManager and UnitOfWork in one transaction unless they share the DbContext instance.
            // Assuming standard ASP.NET Identity setup, they likely share the same AppDbContext if scoped correctly.
            // So I can use `_unitOfWork`'s context to begin a transaction?
            // But `IUnitOfWork` doesn't expose context or transaction.
            
            // I will proceed with the logic as is but keep it cleaner. Refactoring is the main goal.
            
            var newUser = _mapper.Map<AppUser>(addMemberDTO);
            var result = await _userManager.CreateAsync(newUser, addMemberDTO.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ApiResponse.FailureResponse("Failed to create user", errors);
            }

            try 
            {
                await _userManager.AddToRoleAsync(newUser, addMemberDTO.Role.ToString());

                var organizationUser = new OrganizationUser
                {
                    OrganizationId = addMemberDTO.OrganizationId,
                    UserId = newUser.Id,
                    Role = addMemberDTO.Role
                };

                await _unitOfWork.OrganizationUsers.AddAsync(organizationUser);
                await _unitOfWork.SaveAsync();
            }
            catch(Exception)
            {
                // Rollback user creation - primitive manual rollback
                await _userManager.DeleteAsync(newUser);
                throw; // Or return failure
            }

            return ApiResponse.SuccessResponse("Member added successfully",
                new { newUser.Id, newUser.Email, organization.OrganizationId });
        }
    }
}

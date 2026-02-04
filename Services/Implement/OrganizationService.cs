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

        public OrganizationService(UserManager<AppUser> userManager,IUnitOfWork unitOfWork,
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
                            return ApiResponse.FailureResponse("Failed to generate unique organization code. Please try again.", new List<string> { "Unique code generation failed after max retries" });
                        }

                        organization.OrganizationCode = code;

                        try
                        {
                            await _unitOfWork.Organizations.AddAsync(organization);
                            

                            var organizationUser = new OrganizationUser
                            {
                                Organization = organization,
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
                            return ApiResponse.FailureResponse("An error occurred while creating organization.", new List<string> { ex.Message });
                        }
                    }

                }
            }
            return ApiResponse.FailureResponse("Unauthorized", new List<string> { "User is not authorized." });
        }

        public async Task<ApiResponse> AddMemberAsync(AddMemberDTO addMemberDTO, string userId)
        {
            var isMember = await _unitOfWork.Organizations.ValidateUser(addMemberDTO.OrganizationId, userId);
            if (!isMember)
            {
                return ApiResponse.FailureResponse("Unauthorized access to organization.", new List<string> { "User is not a member of the organization." });
            }
            var existingUser = await _userManager.FindByEmailAsync(addMemberDTO.Email);
            if (existingUser != null)
            {
                return ApiResponse.FailureResponse("User creation failed.", new List<string> { "User with this email already exists." });
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(addMemberDTO.OrganizationId);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization Id" });
            }
  
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
                
                await _userManager.DeleteAsync(newUser);
                throw;
            }

            return ApiResponse.SuccessResponse("Member added successfully",
                new { newUser.Id, newUser.Email, organization.OrganizationId });
        }
        public async Task<ApiResponse> GetOrganizationByIdAsync(int organizationId, string userId)
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
            var organizationResponse = _mapper.Map<OrganizationResponseDTO>(organization);
            return ApiResponse.SuccessResponse("Organization retrieved successfully", organizationResponse);
        }
    
        public async Task<ApiResponse> UpdateOrganizationAsync(int organizationId, UpdateOrganizationDTO updateOrganizationDTO, string userId)
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

            _mapper.Map(updateOrganizationDTO, organization);
            
            _unitOfWork.Organizations.Update(organization);
            await _unitOfWork.SaveAsync();

            var organizationResponse = _mapper.Map<OrganizationResponseDTO>(organization);
            return ApiResponse.SuccessResponse("Organization updated successfully", organizationResponse);
        }
        public async Task<ApiResponse> DeleteOrganizationAsync(int organizationId, string userId)
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

            _unitOfWork.Organizations.Delete(organization);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Organization deleted successfully");
        }

        public async Task<ApiResponse> GetUserOrganizationsAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var organizations = await _unitOfWork.Organizations
                    .GetUserOrganizationsAsync(userId);
                var organizationResponses = _mapper
                    .Map<List<OrganizationResponseDTO>>(organizations);
                if(organizations.Count == 0)
                {
                    return ApiResponse.SuccessResponse("User is not part of any organizations yet.", organizationResponses);
                }
                return ApiResponse.SuccessResponse("User organizations retrieved successfully", organizationResponses);
            }
            return ApiResponse.FailureResponse("User not found.", new List<string> { "Invalid User ID" });
        }

        public async Task<ApiResponse> GetOrganizationUsersAsync(int organizationId, string userId)
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

            var users = await _unitOfWork.Organizations.GetOrganizationUsersAsync(organizationId);
            var userResponses = _mapper.Map<List<UserResponseDTO>>(users);

            return ApiResponse.SuccessResponse("Organization users retrieved successfully", userResponses);
        }
    }
}

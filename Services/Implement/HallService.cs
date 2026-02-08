using AutoMapper;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models;
using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public class HallService : IHallService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOrganizationEventService _eventService;

        public HallService(IUnitOfWork unitOfWork, IMapper mapper, IOrganizationEventService eventService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _eventService = eventService;
        }

        public async Task<ApiResponse> CreateHallAsync(HallDTO hallDTO)
        {
            var hall = await _unitOfWork.Halls.GetHallByNameAsync(hallDTO.HallName, hallDTO.OrganizationId);
            if (hall != null)
            {
                return ApiResponse.FailureResponse("Hall with this name already exists.", new List<string> { "Duplicate Hall Name" });
            }

            var organization = await _unitOfWork.Organizations.GetByIdAsync(hallDTO.OrganizationId);
            if (organization == null)
            {
                return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization ID" });
            }

            var hallEntity = _mapper.Map<Hall>(hallDTO);
            await _unitOfWork.Halls.AddAsync(hallEntity);
            await _unitOfWork.SaveAsync();
    
            await _eventService.LogEventAsync(hallDTO.OrganizationId, null, "Hall Created", $"Hall '{hallEntity.HallName}' created.");

            var hallResponse = _mapper.Map<HallResponseDTO>(hallEntity);
            return ApiResponse.SuccessResponse("Hall created successfully.", hallResponse);
        }

        public async Task<ApiResponse> GetAllHallsAsync(int organizationId)
        {
             var organization = await _unitOfWork.Organizations.GetByIdAsync(organizationId);
             if (organization == null)
             {
                 return ApiResponse.FailureResponse("Organization not found.", new List<string> { "Invalid Organization ID" });
             }

             var halls = await _unitOfWork.Halls.GetHallsByOrganizationIdAsync(organizationId);
             var hallResponses = _mapper.Map<List<HallResponseDTO>>(halls);
             
             return ApiResponse.SuccessResponse("Halls retrieved successfully", hallResponses);
        }

        public async Task<ApiResponse> GetHallByIdAsync(int hallId)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(hallId);
            if (hall == null)
            {
                return ApiResponse.FailureResponse("Hall not found", new List<string> { "Invalid Hall ID" });
            }
            var hallResponse = _mapper.Map<HallResponseDTO>(hall);
            return ApiResponse.SuccessResponse("Hall retrieved successfully", hallResponse);
        }

        public async Task<ApiResponse> UpdateHallAsync(int hallId, UpdateHallDTO updateHallDTO)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(hallId);
            if (hall == null)
            {
                return ApiResponse.FailureResponse("Hall not found", new List<string> { "Invalid Hall ID" });
            }
            
            // Check for unique name collision if name is changed
            if (hall.HallName != updateHallDTO.HallName)
            {
                var existingHall = await _unitOfWork.Halls.GetHallByNameAsync(updateHallDTO.HallName, hall.OrganizationId);
                if (existingHall != null)
                {
                    return ApiResponse.FailureResponse("Hall with this name already exists.", new List<string> { "Duplicate Hall Name" });
                }
            }

            _mapper.Map(updateHallDTO, hall);
            hall.UpdatedAt = DateTime.UtcNow;
            
            _unitOfWork.Halls.Update(hall);
            await _unitOfWork.SaveAsync();

            var hallResponse = _mapper.Map<HallResponseDTO>(hall);
            return ApiResponse.SuccessResponse("Hall updated successfully", hallResponse);
        }

        public async Task<ApiResponse> DeleteHallAsync(int hallId)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(hallId);
            if (hall == null)
            {
                return ApiResponse.FailureResponse("Hall not found", new List<string> { "Invalid Hall ID" });
            }

            _unitOfWork.Halls.Delete(hall);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Hall deleted successfully");
        }
    }
}

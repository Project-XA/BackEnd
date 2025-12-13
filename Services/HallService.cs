using AutoMapper;
using Models;
using Project_X.Data.UnitOfWork;
using Project_X.Models.DTOs;
using Project_X.Models.Response;

namespace Project_X.Services
{
    public class HallService : IHallService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HallService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ApiResponse> CreateHallAsync(HallDTO hallDTO)
        {
            var hall = await _unitOfWork.Halls.GetHallByNameAsync(hallDTO.HallName);
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
            hallEntity.Organization = organization;
            await _unitOfWork.Halls.AddAsync(hallEntity);
            await _unitOfWork.SaveAsync();

            return ApiResponse.SuccessResponse("Hall created successfully.");
        }
    }
}

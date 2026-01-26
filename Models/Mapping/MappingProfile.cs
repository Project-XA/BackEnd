using AutoMapper;
using Models;
using Project_X.Models.DTOs;
using Project_X.Models.Enums;

namespace Project_X.Models.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
           CreateMap<UserRegisterDTO,AppUser>()
                .ForMember(dest=>dest.CreatedAt,opt=>opt.MapFrom(src=>DateTime.UtcNow));

            CreateMap<AppUser, UserRegisterDTO>();

            CreateMap<Organization, CreateOrganizationDTO>();

            CreateMap<CreateOrganizationDTO, Organization>()
                .ForMember(dest=>dest.CreatedAt,opt=>opt.MapFrom(src=>DateTime.UtcNow));

            CreateMap<AddMemberDTO,AppUser>().
                ForMember(dest=>dest.CreatedAt,opt=>opt.MapFrom(src=>DateTime.UtcNow));
            
            CreateMap<AppUser, AddMemberDTO>();

            CreateMap<Organization, OrganizationResponseDTO>();
            CreateMap<HallDTO, Hall>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src =>DateTime.UtcNow));
            CreateMap<Hall, HallDTO>().ReverseMap();
            CreateMap<Hall, HallResponseDTO>().ReverseMap();
            CreateMap<Hall, UpdateHallDTO>().ReverseMap();
            CreateMap<AttendanceSession, CreateSessionDTO>().ReverseMap();
            CreateMap<AppUser, UserResponseDTO>().ReverseMap();
            CreateMap<UpdateOrganizationDTO, Organization>();
            CreateMap<UpdateSessionDTO, AttendanceSession>();
            CreateMap<UpdateSessionDTO, AttendanceSession>();
            CreateMap<AttendanceSession, SessionResponseDTO>();
            CreateMap<CreateVerificationSessionDTO, VerificationSession>()
                .ForMember(dest => dest.TimeStamp, opt => opt.MapFrom(src => DateTime.UtcNow));
            
            CreateMap<AttendanceLog, AttendanceRecordDTO>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.VerificationType, opt => opt.MapFrom(src => src.VerificationSession != null ? src.VerificationSession.VerificationType : (VerificationType?)null))
                .ForMember(dest => dest.MatchScore, opt => opt.MapFrom(src => src.VerificationSession != null ? src.VerificationSession.MatchScore : (double?)null));
        }
    }
}

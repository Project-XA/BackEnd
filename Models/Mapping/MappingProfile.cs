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
            CreateMap<OrganizationUser, OrganizationUserResponseDTO>();
        }
    }
}

using AutoMapper;
using Models;
using Project_X.Models.DTOs;

namespace Project_X.Models.Mapping
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<AppUser,UserRegisterDTO>().ReverseMap()
                .ForMember(dest=>dest.CreatedAt,opt=>opt
                .MapFrom(src=> DateTime.Now));

            CreateMap<Organization,CreateOrganizationDTO>().ReverseMap()
                .ForMember(dest=>dest.CreatedAt,opt=>opt
                .MapFrom(src=> DateTime.Now));
        }
    }
}

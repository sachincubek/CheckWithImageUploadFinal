using AutoMapper;
using BookFinalAPI.DTOs;
using BookFinalAPI.Models;

namespace BookFinalAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ApplicationUser → UserDto
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // roles usually loaded separately

            // UserDto → ApplicationUser
            CreateMap<UserDto, ApplicationUser>()
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.Mobile))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Mobile)) // if login is by mobile
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id handled by Identity
                .ForMember(dest => dest.ProfileImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.IsPaid, opt => opt.Ignore());
        }
    }
}

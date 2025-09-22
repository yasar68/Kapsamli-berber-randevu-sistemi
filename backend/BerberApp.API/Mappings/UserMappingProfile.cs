using AutoMapper;
using BerberApp.API.DTOs.Auth;
using BerberApp.API.DTOs.User;
using BerberApp.API.Models;

namespace BerberApp.API.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User modelinden UserDto'ya dönüşüm
            CreateMap<User, UserDto>();

            // UpdateUserDto'dan User modeline dönüşüm
            CreateMap<UpdateUserDto, User>()
                // Password hashing ve salt işleme genellikle servis katmanında yapılır,
                // mapping sırasında yapılmaz.
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // RegisterDto'dan User modeline dönüşüm
            CreateMap<RegisterDto, User>()
                // Burada PasswordHash ve PasswordSalt oluşturulmaz, servis katmanında yapılmalı.
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}

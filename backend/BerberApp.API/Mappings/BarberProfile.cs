using AutoMapper;
using BerberApp.API.Models;
using BerberApp.API.DTOs.Barber;
using System.Collections.Generic;
using System.Linq;

namespace BerberApp.API.Profiles
{
    public class BarberProfile : Profile
    {
        public BarberProfile()
        {
            // Barber -> BarberViewDto
            CreateMap<Barber, BarberViewDto>()
                .ForMember(dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FullName, 
                    opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, 
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, 
                    opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Specialties, 
                    opt => opt.MapFrom(src => src.Specialties)); // Dikkat: Specialties -> Specialties
            
            // CreateBarberDto -> Barber
            CreateMap<CreateBarberDto, Barber>()
                .ForMember(dest => dest.FullName, 
                    opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, 
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, 
                    opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Specialties, 
                    opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dest => dest.CreatedAt, 
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UpdatedAt, 
                    opt => opt.Ignore())
                .ForMember(dest => dest.Appointments,
                    opt => opt.Ignore());

            // UpdateBarberDto -> Barber
            CreateMap<UpdateBarberDto, Barber>()
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore())
                .ForMember(dest => dest.FullName, 
                    opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, 
                    opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, 
                    opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Specialties, 
                    opt => opt.MapFrom(src => src.Specialties))
                .ForMember(dest => dest.UpdatedAt, 
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.CreatedAt, 
                    opt => opt.Ignore())
                .ForMember(dest => dest.Appointments,
                    opt => opt.Ignore());
        }
    }
}
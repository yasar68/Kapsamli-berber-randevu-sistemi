using AutoMapper;
using BerberApp.API.Models;
using BerberApp.API.DTOs.Appointment;
using BerberApp.API.Repositories.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BerberApp.API.Mappings
{
    public class AppointmentProfile : Profile
    {
        private readonly IServiceRepository? _serviceRepository;

        public AppointmentProfile()
        {
            ConfigureBasicMappings();
        }

        public AppointmentProfile(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
            ConfigureBasicMappings();
            ConfigureDependentMappings();
        }

        private void ConfigureBasicMappings()
        {
            CreateMap<Appointment, AppointmentViewDto>()
                .ForMember(dest => dest.ServiceNames, opt => opt.MapFrom(src =>
                    src.AppointmentServices != null && src.AppointmentServices.Any()
                        ? src.AppointmentServices.Select(x => x.Service != null ? x.Service.Name : "Bilinmeyen Servis").ToList()
                        : new List<string> { "Hizmet Yok" }))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src =>
                    src.Barber != null ? src.Barber.FullName : "Berber Yok"))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => GetCustomerDisplayName(src.User)))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.AppointmentDate))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.AppointmentDate.AddMinutes(src.DurationMinutes)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));

            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => EnsureUtc(src.StartTime)))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => (int)(src.EndTime - src.StartTime).TotalMinutes))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note ?? string.Empty))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }

        private void ConfigureDependentMappings()
        {
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => EnsureUtc(src.StartTime)))
                .ForMember(dest => dest.DurationMinutes, opt => opt.Ignore()) // Duration servisten hesaplanacak
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Confirmed))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note ?? string.Empty))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.BarberId, opt => opt.MapFrom(src => src.BarberId))
                .AfterMap(async (src, dest) =>
                {
                    if (_serviceRepository != null && src.ServiceIds != null && src.ServiceIds.Any())
                    {
                        decimal totalPrice = 0;
                        int totalDuration = 0;
                        foreach (var serviceId in src.ServiceIds)
                        {
                            var service = await _serviceRepository.GetByIdAsync(serviceId);
                            if (service != null)
                            {
                                totalPrice += service.Price;
                                totalDuration += service.DurationMinutes;
                            }
                        }
                        dest.Price = totalPrice;
                        dest.DurationMinutes = totalDuration > 0 ? totalDuration : 30;
                    }
                });
        }

        private static string GetCustomerDisplayName(User? user)
        {
            if (user == null) return "Misafir";
            if (!string.IsNullOrWhiteSpace(user.FullName)) return user.FullName;
            if (!string.IsNullOrWhiteSpace(user.Email)) return user.Email.Split('@')[0];
            return "Misafir";
        }

        private static DateTime EnsureUtc(DateTime date)
        {
            return date.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(date, DateTimeKind.Utc)
                : date.ToUniversalTime();
        }
    }
}

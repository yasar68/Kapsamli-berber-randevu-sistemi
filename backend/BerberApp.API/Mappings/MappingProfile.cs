using AutoMapper;
using BerberApp.API.Models;
using BerberApp.API.DTOs.Appointment;
using BerberApp.API.DTOs.Auth;
using BerberApp.API.DTOs.Barber;
using BerberApp.API.DTOs.Report;
using BerberApp.API.DTOs.Review;
using BerberApp.API.DTOs.Service;
using BerberApp.API.DTOs.User;
using BerberApp.API.Repositories.Interfaces;
using System;
using System.Linq;
using System.Collections.Generic;

namespace BerberApp.API.Mappings
{
    public class MappingProfile : Profile
    {
        private readonly IServiceRepository? _serviceRepository;

        public MappingProfile()
        {
            ConfigureUserMappings();
            ConfigureBarberMappings();
            ConfigureServiceMappings();
            ConfigureReviewMappings();
            ConfigureAppointmentMappings();
        }

        public MappingProfile(IServiceRepository serviceRepository) : this()
        {
            _serviceRepository = serviceRepository ?? throw new ArgumentNullException(nameof(serviceRepository));
            ConfigureDependentAppointmentMappings();
        }

        private void ConfigureUserMappings()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src =>
                    !string.IsNullOrWhiteSpace(src.FullName) ? src.FullName : src.UserName));

            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}".Trim()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateUserDto, User>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }

        private void ConfigureBarberMappings()
        {
            CreateMap<Barber, BarberViewDto>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src =>
                    src.Specialties ?? new List<string>()));

            CreateMap<CreateBarberDto, Barber>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom(src =>
                    src.Specialties ?? new List<string>()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateBarberDto, Barber>()
                .ForMember(dest => dest.Specialties, opt => opt.MapFrom((src, dest) =>
                {
                    if (src.Specialties == null || src.Specialties.Count == 0)
                        return dest.Specialties;

                    return src.Specialties.Distinct().ToList();
                }))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }

        private void ConfigureServiceMappings()
        {
            CreateMap<Service, ServiceViewDto>();
            CreateMap<CreateServiceDto, Service>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<UpdateServiceDto, Service>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }

        private void ConfigureReviewMappings()
        {
            CreateMap<Review, ReviewViewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    src.User != null ? src.User.FullName ?? src.User.UserName : "Anonim"));

            CreateMap<CreateReviewDto, Review>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<UpdateReviewDto, Review>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());
        }

        private void ConfigureAppointmentMappings()
        {
            // Çoklu servis destekli dönüşüm
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore()) // Servis katmanında hesaplanacak
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => AppointmentStatus.Confirmed))
                // ServiceIds listesini join tablosu AppointmentServices'e çevir
                .ForMember(dest => dest.AppointmentServices, opt => opt.MapFrom(src =>
                    src.ServiceIds.Select(serviceId => new AppointmentService { ServiceId = serviceId }).ToList()));

            CreateMap<UpdateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => (int)(src.EndTime - src.StartTime).TotalMinutes))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src =>
                    string.IsNullOrWhiteSpace(src.Note) ? null : src.Note))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CustomerId))
                // Update'te servis değişikliği varsa ayrı işlem gerekebilir, burada sadece var olanı ignore ediyoruz
                .ForMember(dest => dest.AppointmentServices, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.BarberId, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.Ignore())
                .ForMember(dest => dest.Status, opt => opt.Ignore());

            CreateMap<Appointment, AppointmentViewDto>()
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.AppointmentDate))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.AppointmentDate.AddMinutes(src.DurationMinutes)))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src =>
                    src.User != null
                        ? (!string.IsNullOrWhiteSpace(src.User.FullName) ? src.User.FullName : src.User.UserName)
                        : "Misafir"))
                .ForMember(dest => dest.BarberName, opt => opt.MapFrom(src =>
                    src.Barber != null
                        ? (!string.IsNullOrWhiteSpace(src.Barber.FullName) ? src.Barber.FullName : "Bilinmeyen Berber")
                        : "Bilinmeyen Berber"))
                // Çoklu servis isimleri listesi olarak dönüyor
                .ForMember(dest => dest.ServiceNames, opt => opt.MapFrom(src =>
                    src.AppointmentServices != null && src.AppointmentServices.Any()
                        ? src.AppointmentServices.Select(x => x.Service != null ? x.Service.Name : "Bilinmeyen Servis").ToList()
                        : new List<string>()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes));
        }

        private void ConfigureDependentAppointmentMappings()
        {
            CreateMap<CreateAppointmentDto, Appointment>()
                .ForMember(dest => dest.AppointmentDate, opt => opt.MapFrom(src => EnsureUtc(src.StartTime)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => AppointmentStatus.Confirmed))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .AfterMap(async (src, dest) =>
                {
                    if (_serviceRepository != null && src.ServiceIds != null && src.ServiceIds.Count > 0)
                    {
                        // Çoklu servislerin toplam fiyatını hesapla
                        decimal totalPrice = 0m;
                        foreach (var serviceId in src.ServiceIds)
                        {
                            var service = await _serviceRepository.GetByIdAsync(serviceId);
                            if (service != null)
                                totalPrice += service.Price;
                        }
                        dest.Price = totalPrice;
                    }
                });
        }

        private static string GetCustomerDisplayName(User user)
        {
            if (user == null) return "Misafir";
            if (!string.IsNullOrWhiteSpace(user.FullName)) return user.FullName;
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var parts = user.Email.Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                return parts.Length > 0 ? parts[0] : "Misafir";
            }
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

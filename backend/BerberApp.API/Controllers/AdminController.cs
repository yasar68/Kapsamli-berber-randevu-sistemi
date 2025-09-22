using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.DTOs.Barber;
using BerberApp.API.DTOs.WorkingHour;
using BerberApp.API.DTOs.User;
using BerberApp.API.DTOs.Appointment;
using BerberApp.API.DTOs.Report;



namespace BerberApp.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IBarberService _barberService;
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IReportService _reportService;
        private readonly ILogger<AdminController> _logger;

        private readonly IWorkingHourService _workingHourService;

        public AdminController(
            IBarberService barberService,
            IUserService userService,
            IAppointmentService appointmentService,
            IReportService reportService,
            IWorkingHourService workingHourService,
            ILogger<AdminController> logger)
        {
            _barberService = barberService;
            _userService = userService;
            _appointmentService = appointmentService;
            _reportService = reportService;
            _workingHourService = workingHourService;
            _logger = logger;
        }
        [Authorize(Roles = "AllowAnonymous")]
        [HttpGet("working-hours/{barberId}")]
        public async Task<IActionResult> GetWorkingHours(int barberId)
        {
            var hours = await _workingHourService.GetByBarberIdAsync(barberId);
            return Ok(hours);
        }

        [HttpPost("working-hours")]
        public async Task<IActionResult> AddWorkingHour([FromBody] WorkingHourDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _workingHourService.AddDtoAsync(dto);
            return Ok(new { Message = "Çalışma saati eklendi" });
        }

        [HttpPut("working-hours")]
        public async Task<IActionResult> UpdateWorkingHour([FromBody] WorkingHourDto dto)
        {
            await _workingHourService.UpdateDtoAsync(dto);
            return Ok(new { Message = "Çalışma saati güncellendi" });
        }

        [HttpDelete("working-hours/{id}")]
        public async Task<IActionResult> DeleteWorkingHour(int id)
        {
            await _workingHourService.DeleteByIdAsync(id);
            return Ok(new { Message = "Çalışma saati silindi" });
        }


        // ✅ Berber Ekleme
        [HttpPost("barbers")]
        public async Task<IActionResult> AddBarber([FromBody] CreateBarberDto dto)
        {
            var result = await _barberService.CreateBarberAsync(dto);
            return Ok(result);
        }

        // ✅ Berber Silme
        [HttpDelete("barbers/{id}")]
        public async Task<IActionResult> DeleteBarber(int id)
        {
            var result = await _barberService.DeleteBarberAsync(id);
            return Ok(new { Success = result });
        }

        // ✅ Tüm Kullanıcıları Getir
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // ✅ Randevuları Tarihe Göre Getir (bugün/hafta/ay)
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointmentsByDate([FromQuery] string period)
        {
            DateTime startDate = DateTime.Today;
            DateTime endDate = DateTime.Today;

            switch (period?.ToLower())
            {
                case "today":
                    endDate = startDate.AddDays(1);
                    break;
                case "week":
                    int diff = (7 + (startDate.DayOfWeek - DayOfWeek.Monday)) % 7;
                    startDate = startDate.AddDays(-1 * diff);
                    endDate = startDate.AddDays(7);
                    break;
                case "month":
                    startDate = new DateTime(startDate.Year, startDate.Month, 1);
                    endDate = startDate.AddMonths(1);
                    break;
                default:
                    return BadRequest("Geçersiz period: today, week veya month olmalı");
            }

            var appointments = await _appointmentService.GetAppointmentsByDateRangeAsync(startDate, endDate);
            return Ok(appointments);
        }

        // ✅ Randevuları duruma göre filtrele
        [HttpGet("appointments/status")]
        public async Task<IActionResult> GetAppointmentsByStatus([FromQuery] string status)
        {
            var result = await _appointmentService.GetAppointmentsAsync(null, null, status);
            return Ok(result);
        }

        // ✅ Rapor Oluştur
        [HttpGet("report")]
        public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var dto = new ReportRequestDto
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var report = await _reportService.GenerateReportAsync(dto);
            return Ok(report);
        }

        // ✅ Kullanıcı Sil
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            return Ok(result);
        }
    }
}

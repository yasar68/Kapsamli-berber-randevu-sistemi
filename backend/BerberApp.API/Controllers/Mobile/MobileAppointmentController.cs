using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.DTOs.Appointment;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Mobile
{
    [ApiController]
    [Route("api/mobile/appointments")]
    public class MobileAppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<MobileAppointmentController> _logger;

        public MobileAppointmentController(IAppointmentService appointmentService, ILogger<MobileAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        // Tüm randevular (opsiyonel: küçük veri yapısı ile)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _appointmentService.GetAllAppointmentsAsync();

                var minimal = result.Select(r => new
                {
                    r.Id,
                    r.StartTime,
                    r.EndTime,
                    r.Status
                });

                return Ok(minimal);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Tüm randevular alınırken hata oluştu.");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // ID ile randevu getir
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var result = await _appointmentService.GetAppointmentByIdAsync(id);
                return result != null ? Ok(result) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Mobil: Randevu ID {id} alınırken hata.");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Belirli müşterinin randevuları
        [HttpGet("by-customer/{customerId:int}")]
        public async Task<IActionResult> GetByCustomer(int customerId, [FromQuery] int? barberId = null, [FromQuery] string status = "all")
        {
            try
            {
                var result = await _appointmentService.GetAppointmentsAsync(customerId, barberId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Müşteri randevuları alınamadı.");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Randevu oluştur
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();

                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return Ok(new { Message = "Oluşturuldu", result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Randevu oluşturulamadı.");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Randevu güncelle
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.UpdateAppointmentAsync(dto);
                return result == null ? NotFound() : Ok(new { Message = "Güncellendi", result.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Güncelleme hatası");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Randevu sil
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _appointmentService.DeleteAppointmentAsync(id);
                return success ? Ok(new { Message = "Silindi" }) : NotFound(new { Message = "Bulunamadı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Silme hatası");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        // Uygun saat dilimleri
        [HttpGet("available-timeslots")]
        public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int barberId, [FromQuery] DateTime date)
        {
            try
            {
                var result = await _appointmentService.GetAvailableTimeSlotsAsync(barberId, date);
                return Ok(result.Select(s => new { Start = s.Start.ToString(@"hh\:mm"), End = s.End.ToString(@"hh\:mm") }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Mobil: Zaman dilimi hatası");
                return StatusCode(500, "Sunucu hatası");
            }
        }
    }
}

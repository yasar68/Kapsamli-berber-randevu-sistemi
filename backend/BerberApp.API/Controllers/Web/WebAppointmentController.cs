using BerberApp.API.DTOs.Appointment;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BerberApp.API.Models;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Web
{
    [ApiController]
    [Route("api/web/appointments")]
    public class WebAppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<WebAppointmentController> _logger;


        public WebAppointmentController(
            IAppointmentService appointmentService,
            ILogger<WebAppointmentController> logger)
        {
            _appointmentService = appointmentService;
            _logger = logger;
        }

        /// <summary>
        /// Tüm randevuları listeler
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var appointments = await _appointmentService.GetAllAppointmentsAsync();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm randevular getirilirken bir hata oluştu");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu", Details = ex.Message });
            }
        }

        /// <summary>
        /// ID'ye göre randevu detayını getirir
        /// </summary>
        /// <param name="id">Randevu ID</param>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
                return appointment != null ? Ok(appointment) : NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} olan randevu getirilirken bir hata oluştu");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu", Details = ex.Message });
            }
        }
        /// <summary>
        /// Müşteri ID ve isteğe bağlı durum filtreli randevuları getirir.
        /// </summary>
        /// <param name="customerId">Müşteri ID</param>
        /// <param name="status">İsteğe bağlı durum filtresi (örnek: "all", "pending", "confirmed", "cancelled", "completed")</param>
        [HttpGet("by-customer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByCustomerId(
    [FromQuery] int? barberId = null,
    [FromQuery] string? status = "all")
        {
            try
            {
                // Token'dan kullanıcı ID'si alınıyor
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                int customerId = int.Parse(userIdClaim.Value);

                var appointments = await _appointmentService.GetAppointmentsAsync(customerId, barberId, status);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Müşteri için randevular getirilirken hata oluştu");
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Sunucu hatası",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Yeni randevu oluşturur
        /// </summary>
        /// <param name="dto">Randevu bilgileri</param>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                    return Unauthorized();

                dto.CustomerId = int.Parse(userIdClaim.Value);

                var result = await _appointmentService.CreateAppointmentAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Veritabanı hatası");
                return BadRequest(new { Message = "Geçersiz kullanıcı, berber veya servis referansı" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu oluşturulurken bir hata oluştu");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu", Details = ex.Message });
            }
        }


        /// <summary>
        /// Randevu bilgilerini günceller
        /// </summary>
        /// <param name="dto">Güncellenecek randevu bilgileri</param>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAppointmentDto dto)
        {
            try
            {
                var result = await _appointmentService.UpdateAppointmentAsync(dto);
                if (result == null) return NotFound();

                return Ok(new
                {
                    Message = "Randevu başarıyla güncellendi",
                    UpdatedFields = new[] { "Tarih", "Süre", "Notlar", "Kullanıcı", "Hizmet" },
                    Appointment = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Randevu güncelleme hatası");
                return StatusCode(500, "Bir hata oluştu");
            }
        }



        /// <summary>
        /// Randevuyu siler
        /// </summary>
        /// <param name="id">Silinecek randevu ID</param>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _appointmentService.DeleteAppointmentAsync(id);

                if (deleted)
                {
                    return Ok(new ApiResponse
                    {
                        Success = true,
                        Message = $"ID {id} olan randevu başarıyla silindi",
                        Data = null
                    });
                }

                return NotFound(new ApiResponse
                {
                    Success = false,
                    Message = $"ID {id} olan randevu bulunamadı",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} olan randevu silinirken bir hata oluştu");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Randevu silinirken bir hata oluştu",
                    ErrorDetails = ex.Message
                });
            }
        }
        [HttpGet("available-timeslots")]
        public async Task<IActionResult> GetAvailableTimeSlots([FromQuery] int barberId, [FromQuery] DateTime date)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableTimeSlotsAsync(barberId, date);
                return Ok(slots.Select(s => new { Start = s.Start.ToString(@"hh\:mm"), End = s.End.ToString(@"hh\:mm") }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Boş zaman dilimleri alınırken hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası", Details = ex.Message });
            }
        }
    }
}
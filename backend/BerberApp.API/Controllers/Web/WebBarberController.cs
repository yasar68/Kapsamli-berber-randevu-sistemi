using BerberApp.API.DTOs.Barber;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Web
{
    [ApiController]
    [Route("api/web/[controller]")]
    public class WebBarberController : ControllerBase
    {
        private readonly IBarberService _barberService;
        private readonly ILogger<WebBarberController> _logger;

        public WebBarberController(
            IBarberService barberService,
            ILogger<WebBarberController> logger)
        {
            _barberService = barberService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var barbers = await _barberService.GetAllBarbersAsync();
                return Ok(barbers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Tüm berberler getirilirken hata oluştu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var barber = await _barberService.GetBarberByIdAsync(id);
                if (barber == null) return NotFound("Berber bulunamadı");
                
                return Ok(barber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} berber getirilirken hata oluştu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBarberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Geçersiz model durumu {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var createdBarber = await _barberService.CreateBarberAsync(dto);
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = createdBarber.Id }, 
                    createdBarber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Berber oluşturulurken hata oluştu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateBarberDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Geçersiz model durumu {@ModelState}", ModelState);
                    return BadRequest(ModelState);
                }

                var updated = await _barberService.UpdateBarberAsync(dto);
                if (!updated) return NotFound("Güncellenecek berber bulunamadı");
                
                return Ok(new { Message = "Berber başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Berber güncellenirken hata oluştu");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _barberService.DeleteBarberAsync(id);
                if (!deleted) return NotFound("Silinecek berber bulunamadı");
                
                return Ok(new { Message = "Berber başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} berber silinirken hata oluştu");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
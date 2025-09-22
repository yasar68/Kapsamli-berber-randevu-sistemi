using Microsoft.AspNetCore.Authorization;
using BerberApp.API.DTOs.Service;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Web
{
    [ApiController]
    [Route("api/web/webservices")]
    public class WebServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly ILogger<WebServiceController> _logger;

        public WebServiceController(IServiceService serviceService, ILogger<WebServiceController> logger)
        {
            _serviceService = serviceService;
            _logger = logger;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var services = await _serviceService.GetAllAsync();
                return Ok(services);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmetler getirilirken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var service = await _serviceService.GetByIdAsync(id);
                if (service == null) return NotFound("Hizmet bulunamadı");
                return Ok(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} hizmet getirilirken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateServiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var created = await _serviceService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet oluşturulurken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateServiceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updated = await _serviceService.UpdateAsync(dto);
                if (updated == null) return NotFound("Güncellenecek hizmet bulunamadı");

                return Ok(new { Message = "Hizmet başarıyla güncellendi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Hizmet güncellenirken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }

        [Authorize(Roles = "admin")] 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _serviceService.DeleteAsync(id);
                if (!deleted) return NotFound("Silinecek hizmet bulunamadı");
                return Ok(new { Message = "Hizmet başarıyla silindi" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"ID {id} hizmet silinirken hata oluştu");
                return StatusCode(500, "Sunucu hatası");
            }
        }
    }
}
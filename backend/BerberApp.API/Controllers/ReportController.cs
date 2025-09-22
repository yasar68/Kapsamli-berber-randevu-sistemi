using System;
using System.Threading.Tasks;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BerberApp.API.DTOs;
using BerberApp.API.DTOs.Report;

namespace BerberApp.API.Controllers.Web
{
    [ApiController]
    [Route("api/reports")]
    public class WebReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<WebReportController> _logger;

        public WebReportController(IReportService reportService, ILogger<WebReportController> logger)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET api/web/reports?startDate=2025-01-01&endDate=2025-01-31
        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> GetReport(
    [FromQuery] DateTime startDate,
    [FromQuery] DateTime endDate,
    [FromQuery] int barberId = 0, // 0: tüm berberler
    [FromQuery] string? notes = null)
        {
            try
            {
                if (startDate == default || endDate == default)
                    return BadRequest("startDate ve endDate parametreleri zorunludur.");

                if (startDate > endDate)
                    return BadRequest("startDate, endDate'den büyük olamaz.");

                var requestDto = new ReportRequestDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    BarberId = barberId,
                    Notes = notes
                };

                var report = await _reportService.GenerateReportAsync(requestDto);

                if (report == null)
                    return NotFound("Belirtilen kriterlere uygun rapor bulunamadı.");

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor alınırken hata oluştu.");
                return StatusCode(500, "Sunucu hatası oluştu.");
            }
        }
    }
}

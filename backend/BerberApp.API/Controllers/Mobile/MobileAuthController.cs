using BerberApp.API.DTOs.Auth;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Mobile
{
    [ApiController]
    [Route("api/mobile/[controller]")]
    public class MobileAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<MobileAuthController> _logger;

        public MobileAuthController(IAuthService authService, ILogger<MobileAuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Kullanıcı girişi
        /// </summary>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return Unauthorized(new
                    {
                        result.Message
                    });
                }

                return Ok(new
                {
                    Token = result.Token,
                    User = result.User,
                    Message = "Giriş başarılı"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Mobil: Giriş sırasında hata oluştu ({loginDto.Email})");
                return StatusCode(500, new { Message = "Sunucu hatası" });
            }
        }

        /// <summary>
        /// Yeni kullanıcı kaydı
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        result.Message
                    });
                }

                return Ok(new
                {
                    Message = "Kayıt başarılı",
                    Token = result.Token,
                    User = result.User
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Mobil: Kayıt sırasında hata oluştu ({registerDto.Email})");
                return StatusCode(500, new { Message = "Sunucu hatası" });
            }
        }
    }
}

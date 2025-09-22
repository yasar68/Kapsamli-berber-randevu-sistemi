using BerberApp.API.DTOs.Auth;
using BerberApp.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Helpers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BerberApp.API.Controllers.Web
{
    [ApiController]
    [Route("api/web/[controller]")]
    public class WebAuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly JwtHelper _jwtHelper;
        private readonly PasswordHelper _passwordHelper;
        private readonly ILogger<WebAuthController> _logger;


        public WebAuthController(
            IAuthService authService,
            IUserRepository userRepository,
            IUserRoleRepository userRoleRepository,
            JwtHelper jwtHelper,
            PasswordHelper passwordHelper,   // ekledik
            ILogger<WebAuthController> logger)
        {
            _authService = authService;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _jwtHelper = jwtHelper;
            _passwordHelper = passwordHelper;  // atadık
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });

            bool isPasswordValid = _passwordHelper.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt);

            if (!isPasswordValid)
                return Unauthorized(new { message = "Geçersiz e-posta veya şifre." });

            // Kullanıcı rolleri
            var roles = await _userRoleRepository.GetRolesByUserIdAsync(user.Id);

            // Token üret
            var token = _jwtHelper.GenerateToken(user.Id.ToString(), user.Email, roles.ToArray());

            return Ok(new
            {
                Token = token,
                Email = user.Email,
                Roles = roles
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                var result = await _authService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    _logger.LogWarning("Registration failed for user {Email}: {Message}", registerDto.Email, result.Message);
                    return BadRequest(new { message = result.Message });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during registration for user {Email}", registerDto.Email);
                return StatusCode(500, new { message = "Sunucu hatası oluştu." });
            }
        }

    }
}

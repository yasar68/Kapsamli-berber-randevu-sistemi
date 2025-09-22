using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.DTOs.User;
using BerberApp.API.Models;

namespace BerberApp.API.Controllers.Mobile
{
    [ApiController]
    [Route("api/mobile/users")]
    public class MobileUserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<MobileUserController> _logger;

        public MobileUserController(IUserService userService, ILogger<MobileUserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Kullanıcılar listelenirken hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu." });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { Message = "Kullanıcı bulunamadı." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Id={id} ile kullanıcı getirilirken hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu." });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id != updateUserDto.Id)
                return BadRequest(new { Message = "URL id ile gönderilen id uyuşmuyor." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                    return NotFound(new { Message = "Kullanıcı bulunamadı." });

                await _userService.UpdateUserAsync(updateUserDto);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Id={id} ile kullanıcı güncellenirken hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu." });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var existingUser = await _userService.GetUserByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound(new ApiResponse
                    {
                        Success = false,
                        Message = "Kullanıcı bulunamadı."
                    });
                }

                var deleteResult = await _userService.DeleteUserAsync(id);

                if (deleteResult.Success)
                    return Ok(deleteResult);

                return BadRequest(deleteResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Id={id} ile kullanıcı silinirken hata oluştu.");
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Message = "Sunucu hatası oluştu."
                });
            }
        }

        [HttpPost("password-reset/request")]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestDto dto)
        {
            try
            {
                var result = await _userService.SendPasswordResetCodeAsync(dto.Email);
                if (!result.Success)
                    return BadRequest(new { Message = "E-posta adresi sistemde kayıtlı değil." });

                return Ok(new { Message = "Şifre sıfırlama kodu e-posta adresinize gönderildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre sıfırlama kodu gönderilirken hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu." });
            }
        }

        [HttpPost("password-reset/confirm")]
        public async Task<IActionResult> ConfirmPasswordReset([FromBody] PasswordResetConfirmDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _userService.ResetPasswordAsync(dto.Email, dto.Code, dto.NewPassword);
                if (!result.Success)
                    return BadRequest(new { Message = result.Message });

                return Ok(new { Message = "Şifre başarıyla değiştirildi." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Şifre sıfırlama onayı sırasında hata oluştu.");
                return StatusCode(500, new { Message = "Sunucu hatası oluştu." });
            }
        }
    }
}

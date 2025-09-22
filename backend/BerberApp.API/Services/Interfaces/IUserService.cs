using System.Collections.Generic;
using System.Threading.Tasks;
using BerberApp.API.DTOs.User;
using BerberApp.API.Models;

namespace BerberApp.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(int id);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task AddUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(UpdateUserDto updateUserDto);
        Task<ApiResponse> DeleteUserAsync(int id); // Dönüş tipi ApiResponse olarak değiştirildi
        Task<ApiResponse> SendPasswordResetCodeAsync(string email);
        Task<ApiResponse> ResetPasswordAsync(string email, string code, string newPassword);
    }
}

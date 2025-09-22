namespace BerberApp.API.DTOs.User
{
    public class PasswordResetConfirmDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string NewPassword { get; set; } = null!;
    }
}
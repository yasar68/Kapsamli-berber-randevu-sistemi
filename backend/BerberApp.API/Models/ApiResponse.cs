namespace BerberApp.API.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
        public string? ErrorDetails { get; set; }
    }
}
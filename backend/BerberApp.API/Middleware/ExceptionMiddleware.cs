using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BerberApp.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate nextParam, ILogger<ExceptionMiddleware> loggerParam)
        {
            _next = nextParam ?? throw new ArgumentNullException(nameof(nextParam));
            _logger = loggerParam ?? throw new ArgumentNullException(nameof(loggerParam));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // Loglama
                _logger.LogError(ex, "Unhandled exception occurred.");

                // Konsola detaylı yazma
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Exception] {DateTime.Now}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();

                // İstemciye JSON hata yanıtı
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var errorResponse = new
                {
                    message = "Sunucuda bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.",
                    detail = ex.Message // istersen hata mesajını kaldırabilirsin güvenlik açısından
                };

                await context.Response.WriteAsJsonAsync(errorResponse);
            }
        }
    }
}
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BerberApp.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            var request = context.Request;
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            _logger.LogInformation("Request started: {Method} {Path} from IP {IP}", request.Method, request.Path, ip);

            await _next(context);

            stopwatch.Stop();

            var responseStatus = context.Response.StatusCode;

            _logger.LogInformation("Request finished: {Method} {Path} from IP {IP} with status {StatusCode} in {ElapsedMilliseconds}ms",
                request.Method, request.Path, ip, responseStatus, stopwatch.ElapsedMilliseconds);
        }
    }
}

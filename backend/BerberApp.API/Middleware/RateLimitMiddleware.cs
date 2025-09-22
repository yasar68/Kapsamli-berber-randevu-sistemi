using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BerberApp.API.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, RequestCounter> _requests = new();
        private readonly int _maxRequests;
        private readonly TimeSpan _timeWindow;

        public RateLimitMiddleware(RequestDelegate next, ILogger<RateLimitMiddleware> logger, int maxRequests = 60, int secondsWindow = 60)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maxRequests = maxRequests;
            _timeWindow = TimeSpan.FromSeconds(secondsWindow);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var now = DateTime.UtcNow;

            var counter = _requests.GetOrAdd(ipAddress, new RequestCounter
            {
                Timestamp = now,
                Count = 0
            });

            bool rateLimitExceeded;
            string retryAfter = "0";

            lock (counter)
            {
                if (now - counter.Timestamp > _timeWindow)
                {
                    counter.Timestamp = now;
                    counter.Count = 1;
                    rateLimitExceeded = false;
                }
                else
                {
                    counter.Count++;
                    rateLimitExceeded = counter.Count > _maxRequests;
                    
                    if (rateLimitExceeded)
                    {
                        retryAfter = (_timeWindow - (now - counter.Timestamp)).TotalSeconds.ToString("F0");
                    }
                }
            }

            if (rateLimitExceeded)
            {
                _logger.LogWarning("IP {Ip} has exceeded the rate limit.", ipAddress);
                context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
                context.Response.ContentType = "application/json";
                context.Response.Headers["Retry-After"] = retryAfter;

                await context.Response.WriteAsJsonAsync(new
                {
                    message = $"Çok fazla istek gönderdiniz. Lütfen {retryAfter} saniye sonra tekrar deneyiniz."
                });
                return;
            }

            await _next(context);
        }

        private class RequestCounter
        {
            public DateTime Timestamp { get; set; }
            public int Count { get; set; }
        }
    }
}
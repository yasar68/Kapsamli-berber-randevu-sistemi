using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using BerberApp.API.Config;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;

namespace BerberApp.API.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        public AuthenticationMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtSettings, ILogger<AuthenticationMiddleware> logger)
        {
            _next = next;
            _jwtSettings = jwtSettings.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Authorization header is required");
                return;
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid authorization scheme. Expected 'Bearer' token");
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is required");
                return;
            }

            try
            {
                AttachUserToContext(context, token);
                
                // Null kontrolü eklenmiş loglama
                var logger = context.RequestServices.GetService<ILogger<AuthenticationMiddleware>>();
                if (logger != null && logger.IsEnabled(LogLevel.Debug))
                {
                    foreach (var claim in context.User.Claims)
                    {
                        logger.LogDebug($"CLAIM: {claim.Type} = {claim.Value}");
                    }
                }

                await _next(context);
            }
            catch (SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token has expired");
            }
            catch (SecurityTokenValidationException stvex)
            {
                _logger.LogError(stvex, "Token validation failed");
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Authentication middleware error");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("An unexpected error occurred");
            }
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                
                if (!tokenHandler.CanReadToken(token))
                {
                    throw new SecurityTokenValidationException("Token cannot be read");
                }

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                    ValidateIssuer = !string.IsNullOrEmpty(_jwtSettings.Issuer),
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = !string.IsNullOrEmpty(_jwtSettings.Audience),
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = "role"
                };

                var principal = tokenHandler.ValidateToken(token, validationParams, out _);
                
                var identity = principal.Identity as ClaimsIdentity;
                if (identity != null)
                {
                    if (!identity.HasClaim(c => c.Type == "role"))
                    {
                        var roleClaim = identity.Claims.FirstOrDefault(c => 
                            c.Type == ClaimTypes.Role || 
                            c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
                        
                        if (roleClaim != null)
                        {
                            identity.AddClaim(new Claim("role", roleClaim.Value));
                        }
                    }
                }

                context.User = principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed in AttachUserToContext");
                throw;
            }
        }
    }
}
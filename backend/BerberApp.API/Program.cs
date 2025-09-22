using BerberApp.API.Data;
using BerberApp.API.Helpers;
using BerberApp.API.Mappings;
using BerberApp.API.Middleware;
using BerberApp.API.Config;
using BerberApp.API.Repositories;
using BerberApp.API.Services;
using BerberApp.API.Validators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using BerberApp.API.Services.Interfaces;
using BerberApp.API.Services.Implementations;
using BerberApp.API.Repositories.Interfaces;
using BerberApp.API.Repositories.Implementations;
using System.Text.Json.Serialization;
using Hangfire;
using Hangfire.MemoryStorage;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Configuration ------------------
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// ------------------ Configuration Binding ------------------
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

// ------------------ DbContext (SQLite) ------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// ------------------ Hangfire ------------------
builder.Services.AddHangfire(config =>
{
    config.UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseMemoryStorage();
});
builder.Services.AddHangfireServer();

// ------------------ AutoMapper ------------------
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ------------------ FluentValidation ------------------
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

// ------------------ Services & Repositories ------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IBarberService, BarberService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddHostedService<AppointmentReminderService>();
builder.Services.AddScoped<IWorkingHourService, WorkingHourService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<PasswordHelper>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IBarberRepository, BarberRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IWorkingHourRepository, WorkingHourRepository>();
builder.Services.AddScoped<IAppointmentServiceRepository, AppointmentServiceRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();

// ------------------ Authentication (JWT) ------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is missing");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = "role"
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Token'ın başındaki "Bearer " önekini kaldır
            var token = context.Token ?? context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(token) &&
                token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Token = token.Substring("Bearer ".Length).Trim();
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(context.Exception, "Authentication failed");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Token successfully validated");
            return Task.CompletedTask;
        }
    };
});

// ------------------ Authorization ------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
    {
        policy.RequireAuthenticatedUser();
        policy.RequireAssertion(context =>
        {
            var roleClaim = context.User.FindFirst(c => 
                c.Type == "role" || 
                c.Type == ClaimTypes.Role || 
                c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            
            return roleClaim != null && roleClaim.Value == "admin";
        });
    });
});
// ------------------ Controllers & Swagger ------------------
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "BerberApp API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------ CORS ------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ------------------ Uygulama Başlat ------------------
var app = builder.Build();

// ------------------ Middleware Sırası ------------------
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BerberApp API V1");
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}

// Hangfire Dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireDashboardAuthorization() }
});

// Scheduled Jobs
RecurringJob.AddOrUpdate<IWorkingHourService>(
    "auto-add-working-hours",
    service => service.AddDefaultWorkingHoursForTomorrowIfMissing(),
    Cron.Daily(0, 1)
);

// Controller endpoints
app.MapControllers();

// ------------------ Veritabanı Başlatma ------------------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (db.Database.GetPendingMigrations().Any())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Applying pending migrations...");
        db.Database.Migrate();
    }

    UserSeeder.Seed(db);
}

app.Run();
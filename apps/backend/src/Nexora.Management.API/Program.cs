using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nexora.Management.Domain.Entities;
using Nexora.Management.Infrastructure.Authentication;
using Nexora.Management.Infrastructure.Interfaces;
using Nexora.Management.API.Middleware;
using Nexora.Management.Infrastructure.Persistence;
using Nexora.Management.API.Endpoints;
using Nexora.Management.API.Middlewares;
using Nexora.Management.Infrastructure.Services;
using Nexora.Management.Application.Authorization;
using Nexora.Management.Application.Common;
using Nexora.Management.API.Hubs;
using Nexora.Management.API.Services;
using Nexora.Management.API.Common;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Configure JWT Authentication
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    // This will be dynamically handled by the PermissionAuthorizationHandler
    // Policies follow the format: "Permission:resource:action"
    // Example: [RequirePermission("tasks", "create")] generates policy "Permission:tasks:create"
});

// Register Permission Authorization Policy Provider
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();

// Register Permission Authorization Handler (scoped to get DbContext)
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

// Configure CORS with whitelisted origins for JWT security
var corsSettings = new CorsSettings();
builder.Configuration.GetSection(CorsSettings.SectionName).Bind(corsSettings);
builder.Services.AddSingleton(corsSettings);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        // Use whitelisted origins from configuration for security
        // AllowAnyOrigin() would break JWT authentication with credentials
        if (corsSettings.AllowedOrigins.Length > 0)
        {
            policy.WithOrigins(corsSettings.AllowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Required for JWT cookies/headers
        }
        else
        {
            // Fallback: only localhost for development if no config provided
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        }
    });
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Nexora Management API",
        Version = "v1",
        Description = "Team management, task tracking, and project coordination API"
    });
});

// Register Application layer services
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Nexora.Management.Application.Common.ApiResponse<>).Assembly));

// Register Infrastructure layer services
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions
            .MigrationsAssembly("Nexora.Management.API")
            .EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());

    // Ignore pending model changes warning - expected when adding new migrations
    options.ConfigureWarnings(warnings => warnings.Ignore(
        Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
});

// Register Infrastructure interfaces
builder.Services.AddScoped<IAppDbContext>(provider =>
    provider.GetRequiredService<AppDbContext>());

// Register JWT Token Service
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Register Password Hasher
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Register User Context
builder.Services.AddScoped<IUserContext, UserContext>();

// Register File Storage Service
builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

// Register SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
});

// Register Presence and Notification Services
builder.Services.AddScoped<IPresenceService, PresenceService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Auto-apply database migrations
// NOTE: Currently disabled due to pre-existing bug in InitialCreate migration (wrong column name in Comments index)
// TODO: Fix InitialCreate migration and re-enable auto-migration
// try
// {
//     using (var scope = app.Services.CreateScope())
//     {
//         var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//         Log.Information("Applying database migrations...");
//         db.Database.Migrate();
//         Log.Information("Database migrations applied successfully");
//     }
// }
// catch (Exception ex)
// {
//     Log.Fatal(ex, "Failed to apply database migrations");
//     throw;
// }

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Add Workspace Authorization Middleware for RLS support
app.UseWorkspaceAuthorization();

app.MapControllers();

// Map Auth endpoints
app.MapAuthEndpoints();

// Map Workspace endpoints
app.MapWorkspaceEndpoints();

// Map Task endpoints
app.MapTaskEndpoints();

// Map Comment endpoints
app.MapCommentEndpoints();

// Map Attachment endpoints
app.MapAttachmentEndpoints();

// Map Document endpoints
app.MapDocumentEndpoints();

// Map Goal endpoints
app.MapGoalEndpoints();

// Map ClickUp Hierarchy endpoints
app.MapSpaceEndpoints();
app.MapFolderEndpoints();
app.MapTaskListEndpoints();
app.MapTimeEndpoints();
app.MapAnalyticsEndpoints();
app.MapDashboardEndpoints();

// Map SignalR Hubs
app.MapHub<TaskHub>("/hubs/tasks");
app.MapHub<PresenceHub>("/hubs/presence");
app.MapHub<NotificationHub>("/hubs/notifications");

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}))
.WithName("HealthCheck")
.WithOpenApi();

// Welcome endpoint
app.MapGet("/", () => Results.Ok(new
{
    message = "Welcome to Nexora Management API",
    version = "v1",
    documentation = "/swagger"
}))
.WithName("Root")
.WithOpenApi();

try
{
    Log.Information("Starting Nexora Management API");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

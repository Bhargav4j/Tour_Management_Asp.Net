using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Services;

namespace TourManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Build connection string from environment variables or fallback to configuration
        var host = Environment.GetEnvironmentVariable("DB_HOST") ?? configuration["DatabaseSettings:Host"] ?? "localhost";
        var port = Environment.GetEnvironmentVariable("DB_PORT") ?? configuration["DatabaseSettings:Port"] ?? "5432";
        var database = Environment.GetEnvironmentVariable("DB_NAME") ?? configuration["DatabaseSettings:Database"] ?? "tourmanagementdb";
        var username = Environment.GetEnvironmentVariable("DB_USER") ?? configuration["DatabaseSettings:Username"] ?? "postgres";
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? configuration["DatabaseSettings:Password"] ?? "postgres";

        var connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};Include Error Detail=true";

        services.AddDbContext<TourManagementDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                npgsqlOptions => npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null))
            .UseSnakeCaseNamingConvention());

        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<IUserInfoRepository, UserInfoRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // Register file storage service
        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        return services;
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for registering infrastructure services
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Host=localhost;Port=5432;Database=tourmanagementdb;Username=postgres;Password=postgres";

        services.AddDbContext<TourManagementDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorCodesToAdd: null);
                npgsqlOptions.MigrationsHistoryTable("__efmigrations_history", "public");
            })
            .UseSnakeCaseNamingConvention();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        });

        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        return services;
    }
}

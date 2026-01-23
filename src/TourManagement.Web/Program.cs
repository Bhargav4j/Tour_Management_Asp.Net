using Microsoft.EntityFrameworkCore;
using Serilog;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/tourmanagement-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddRazorPages();

// Configure DbContext
builder.Services.AddDbContext<TourManagementDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null);
            npgsqlOptions.MigrationsHistoryTable("__efmigrations_history", "public");
        })
        .UseSnakeCaseNamingConvention()
        .EnableSensitiveDataLogging(builder.Environment.IsDevelopment());

    AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
});

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

try
{
    Log.Information("Starting Tour Management application");
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

using Microsoft.EntityFrameworkCore;
using Serilog;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Application.Services;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
var enableFileLogs = builder.Configuration.GetValue<bool>("Serilog:EnableFileLogging", false);
var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console();

if (enableFileLogs)
{
    loggerConfig.WriteTo.File("logs/tourmanagement-.txt", rollingInterval: RollingInterval.Day);
}

Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddRazorPages();

// Configure Database
builder.Services.AddDbContext<TourManagementDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions
            .EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null)
            .MigrationsHistoryTable("__efmigrations_history", "public"))
    .UseSnakeCaseNamingConvention();
});

// Register Repositories
builder.Services.AddScoped<ITourRepository, TourRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register Services
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim("IsAdmin", "True"));
});

// Configure Session
var redisConnection = builder.Configuration.GetValue<string>("REDIS_CONNECTION");
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "TourManagement_";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Configure Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TourManagementDbContext>();

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

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

// Map Health Check Endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");

try
{
    Log.Information("Starting Tour Management Application");
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

// Make Program class accessible for testing
public partial class Program { }

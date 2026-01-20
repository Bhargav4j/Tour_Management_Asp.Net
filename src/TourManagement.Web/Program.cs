using Serilog;
using TourManagement.Application.Extensions;
using TourManagement.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Explicitly add environment variables for configuration override clarity
builder.Configuration.AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddRazorPages();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddHttpContextAccessor();

// Add health checks for container orchestration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<TourManagement.Infrastructure.Data.TourManagementDbContext>();

// Add distributed cache for session storage (Redis or in-memory for development)
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? builder.Configuration["Redis:Connection"];
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
    // Fallback to in-memory cache for development
    builder.Services.AddDistributedMemoryCache();
}

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

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

// Map health check endpoint for Kubernetes probes
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting Tour Management application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
}
finally
{
    Log.CloseAndFlush();
}

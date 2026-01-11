using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure PostgreSQL with Entity Framework Core
builder.Services.AddDbContext<TourDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        // Enable retry on failure for resilience
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorCodesToAdd: null);

        // Set command timeout
        npgsqlOptions.CommandTimeout(30);

        // Configure migrations history table with snake_case naming
        npgsqlOptions.MigrationsHistoryTable("__ef_migrations_history", "public");
    })
    // Use snake_case naming convention for all database objects
    .UseSnakeCaseNamingConvention()
    // Enable legacy timestamp behavior for compatibility
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment())
    .EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// Configure timestamp behavior globally
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Tour Management API - PostgreSQL Ready!");

app.Run();

public partial class Program { }

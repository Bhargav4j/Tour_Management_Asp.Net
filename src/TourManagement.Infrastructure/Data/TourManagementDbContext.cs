using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data;

/// <summary>
/// Database context for Tour Management application
/// </summary>
public class TourManagementDbContext : DbContext
{
    public TourManagementDbContext(DbContextOptions<TourManagementDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema to public for PostgreSQL
        modelBuilder.HasDefaultSchema("public");

        // Apply entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TourManagementDbContext).Assembly);
    }
}

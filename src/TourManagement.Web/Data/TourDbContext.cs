using Microsoft.EntityFrameworkCore;
using TourManagement.Web.Models;

namespace TourManagement.Web.Data;

public class TourDbContext : DbContext
{
    public TourDbContext(DbContextOptions<TourDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserInfo> UserInfos { get; set; } = null!;
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<Booking> Bookings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set default schema to public (PostgreSQL default)
        modelBuilder.HasDefaultSchema("public");

        // Configure UserInfo entity
        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable(t => t.HasCheckConstraint("CK_Gender", "gender IN ('Male', 'Female')"));
        });

        // Configure Tour entity
        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.TourId);

            entity.Property(e => e.TourId)
                .UseIdentityColumn();

            entity.Property(e => e.Price)
                .HasPrecision(6, 0);
        });

        // Configure Booking entity
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.TourId);

            entity.Property(e => e.TourId)
                .UseIdentityColumn();
        });
    }
}

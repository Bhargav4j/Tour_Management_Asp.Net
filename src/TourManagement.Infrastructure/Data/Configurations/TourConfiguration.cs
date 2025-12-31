using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Tour entity
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");

        builder.HasKey(t => t.TourId);

        builder.Property(t => t.TourId)
            .HasColumnName("TOUR_ID")
            .UseIdentityColumn(1, 1);

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .HasPrecision(6, 0)
            .IsRequired();

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.PicturePath)
            .HasColumnName("pic")
            .HasMaxLength(200);

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

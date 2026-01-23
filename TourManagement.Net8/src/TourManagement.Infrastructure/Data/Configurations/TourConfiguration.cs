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

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TourName)
            .IsRequired()
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(255);

        builder.Property(t => t.Place)
            .IsRequired()
            .HasColumnName("PLACE")
            .HasMaxLength(255);

        builder.Property(t => t.Days)
            .IsRequired()
            .HasColumnName("DAYS");

        builder.Property(t => t.Price)
            .IsRequired()
            .HasColumnName("PRICE")
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Locations)
            .IsRequired()
            .HasColumnName("LOCATIONS")
            .HasMaxLength(500);

        builder.Property(t => t.TourInfo)
            .IsRequired()
            .HasColumnName("TOUR_INFO")
            .HasMaxLength(2000);

        builder.Property(t => t.PictureFileName)
            .HasColumnName("pic")
            .HasMaxLength(255);

        builder.Property(t => t.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(t => t.ModifiedBy)
            .HasMaxLength(100);

        // Navigation properties
        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

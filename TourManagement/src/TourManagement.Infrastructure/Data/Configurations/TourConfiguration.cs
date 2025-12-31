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
            .HasMaxLength(255)
            .HasColumnName("TOUR_NAME");

        builder.Property(t => t.Description)
            .HasMaxLength(1000)
            .HasColumnName("TOUR_DESCRIPTION");

        builder.Property(t => t.Place)
            .IsRequired()
            .HasMaxLength(255)
            .HasColumnName("PLACE");

        builder.Property(t => t.Price)
            .HasColumnType("decimal(18,2)")
            .HasColumnName("PRICE");

        builder.Property(t => t.Duration)
            .HasColumnName("DURATION");

        builder.Property(t => t.ImageUrl)
            .HasMaxLength(500)
            .HasColumnName("IMAGE_URL");

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(100);

        builder.Property(t => t.ModifiedBy)
            .HasMaxLength(100);

        // Relationships
        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

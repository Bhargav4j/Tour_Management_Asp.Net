using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Tour entity
/// </summary>
public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.ToTable("Tour");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("TOUR_ID");

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(t => t.ImageFileName)
            .HasColumnName("pic")
            .HasMaxLength(255);

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("NOW()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(t => t.ModifiedBy)
            .HasMaxLength(100);

        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

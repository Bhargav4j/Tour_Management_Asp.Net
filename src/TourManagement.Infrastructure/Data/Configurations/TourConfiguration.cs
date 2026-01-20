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

        builder.Property(t => t.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(t => t.TourName)
            .HasColumnName("TOUR_NAME")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Place)
            .HasColumnName("PLACE")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Days)
            .HasColumnName("DAYS")
            .IsRequired();

        builder.Property(t => t.Price)
            .HasColumnName("PRICE")
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.Locations)
            .HasColumnName("LOCATIONS")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(t => t.TourInfo)
            .HasColumnName("TOUR_INFO")
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(t => t.PictureFileName)
            .HasColumnName("pic")
            .HasMaxLength(255);

        builder.Property(t => t.CreatedDate)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(t => t.IsActive)
            .HasDefaultValue(true);

        builder.Property(t => t.CreatedBy)
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(t => t.ModifiedBy)
            .HasMaxLength(100);

        builder.HasMany(t => t.Bookings)
            .WithOne(b => b.Tour)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for Booking entity
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("TOUR_ID")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.TourName)
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(50);

        builder.Property(b => b.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(50);

        builder.Property(b => b.UserEmail)
            .HasColumnName("Email")
            .HasMaxLength(50);

        builder.Property(b => b.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(50);

        builder.Property(b => b.BookingDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.ModifiedDate)
            .HasDefaultValueSql("NULL");

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.ModifiedBy)
            .HasMaxLength(100);
    }
}

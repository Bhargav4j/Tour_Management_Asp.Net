using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("booking");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("TOUR_ID")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.TourId)
            .HasColumnName("TourId")
            .IsRequired();

        builder.Property(b => b.TourName)
            .HasColumnName("TOUR_NAME")
            .HasMaxLength(50);

        builder.Property(b => b.Place)
            .HasColumnName("PLACE")
            .HasMaxLength(50);

        builder.Property(b => b.Email)
            .HasColumnName("Email")
            .HasMaxLength(50);

        builder.Property(b => b.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(50);

        builder.Property(b => b.BookingDate)
            .HasColumnName("BookingDate")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);
    }
}

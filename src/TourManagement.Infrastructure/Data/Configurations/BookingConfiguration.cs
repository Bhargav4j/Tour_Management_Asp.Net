using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Booking entity
/// </summary>
public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("BOOKING_ID")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.TourId)
            .HasColumnName("TOUR_ID")
            .IsRequired();

        builder.Property(b => b.UserId)
            .HasColumnName("USER_ID")
            .IsRequired();

        builder.Property(b => b.NumberOfPeople)
            .HasColumnName("NumberOfPeople")
            .IsRequired();

        builder.Property(b => b.TotalAmount)
            .HasColumnName("TotalAmount")
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(b => b.BookingDate)
            .HasColumnName("BookingDate")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.BookingStatus)
            .HasColumnName("BookingStatus")
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(b => b.Notes)
            .HasColumnName("Notes")
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedDate)
            .HasColumnName("CreatedDate")
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.ModifiedDate)
            .HasColumnName("ModifiedDate");

        builder.Property(b => b.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(b => b.ModifiedBy)
            .HasColumnName("ModifiedBy")
            .HasMaxLength(100);

        builder.HasOne(b => b.Tour)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

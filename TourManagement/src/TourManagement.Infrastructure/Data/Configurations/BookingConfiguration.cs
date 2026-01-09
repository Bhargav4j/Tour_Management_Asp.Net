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
        builder.ToTable("Bookings");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.Property(b => b.NumberOfPeople)
            .IsRequired();

        builder.Property(b => b.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(b => b.Status)
            .HasMaxLength(50)
            .IsRequired()
            .HasDefaultValue("Pending");

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("NOW()");

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(b => b.ModifiedBy)
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

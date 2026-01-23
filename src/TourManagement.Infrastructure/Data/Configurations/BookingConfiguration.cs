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
        builder.ToTable("Booking");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.TourId)
            .IsRequired();

        builder.Property(b => b.BookingDate)
            .IsRequired();

        builder.Property(b => b.NumberOfPeople)
            .IsRequired();

        builder.Property(b => b.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.CreatedDate)
            .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

        builder.Property(b => b.IsActive)
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(b => b.ModifiedBy)
            .HasMaxLength(255);

        builder.HasOne(b => b.User)
            .WithMany(u => u.Bookings)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Tour)
            .WithMany(t => t.Bookings)
            .HasForeignKey(b => b.TourId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

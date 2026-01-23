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

        builder.Property(b => b.Id)
            .HasColumnName("BookingId")
            .ValueGeneratedOnAdd();

        builder.Property(b => b.TourId)
            .HasColumnName("TourId")
            .IsRequired();

        builder.Property(b => b.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(b => b.BookingDate)
            .HasColumnName("BookingDate")
            .IsRequired();

        builder.Property(b => b.NumberOfPeople)
            .HasColumnName("NumberOfPeople")
            .IsRequired();

        builder.Property(b => b.TotalPrice)
            .HasColumnName("TotalPrice")
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Status)
            .HasColumnName("Status")
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(b => b.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(b => b.ModifiedDate);

        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(b => b.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

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

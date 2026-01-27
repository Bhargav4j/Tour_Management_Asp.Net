using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework Core configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("UserInfo");

        builder.HasKey(u => u.Email);

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.Gender)
            .HasColumnName("Gender")
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .HasColumnName("Password")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(u => u.DateOfBirth)
            .HasColumnName("dob")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(u => u.Street)
            .HasColumnName("Street")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.City)
            .HasColumnName("City")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.State)
            .HasColumnName("State")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(u => u.CreatedDate)
            .HasColumnName("CreatedDate")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.ModifiedDate)
            .HasColumnName("ModifiedDate");

        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedBy)
            .HasColumnName("CreatedBy")
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(u => u.ModifiedBy)
            .HasColumnName("ModifiedBy")
            .HasMaxLength(100);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.Email)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

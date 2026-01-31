using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for User entity
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("USER_ID")
            .ValueGeneratedOnAdd();

        builder.Property(u => u.Username)
            .HasColumnName("Username")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.PasswordHash)
            .HasColumnName("PasswordHash")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(u => u.FullName)
            .HasColumnName("FullName")
            .HasMaxLength(200);

        builder.Property(u => u.Phone)
            .HasColumnName("Phone")
            .HasMaxLength(50);

        builder.Property(u => u.Address)
            .HasColumnName("Address")
            .HasMaxLength(500);

        builder.Property(u => u.IsAdmin)
            .HasColumnName("IsAdmin")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedDate)
            .HasColumnName("CreatedDate")
            .IsRequired()
            .HasDefaultValueSql("NOW()");

        builder.Property(u => u.ModifiedDate)
            .HasColumnName("ModifiedDate");

        builder.Property(u => u.IsActive)
            .HasColumnName("IsActive")
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedBy)
            .HasColumnName("CreatedBy")
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System");

        builder.Property(u => u.ModifiedBy)
            .HasColumnName("ModifiedBy")
            .HasMaxLength(100);

        builder.HasIndex(u => u.Username)
            .IsUnique();

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

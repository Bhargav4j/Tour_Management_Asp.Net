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
        builder.ToTable("Userinfo");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("email");

        builder.HasIndex(u => u.Email)
            .IsUnique();

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(200)
            .HasColumnName("password");

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .HasColumnName("FirstName");

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .HasColumnName("LastName");

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20)
            .HasColumnName("PhoneNumber");

        builder.Property(u => u.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

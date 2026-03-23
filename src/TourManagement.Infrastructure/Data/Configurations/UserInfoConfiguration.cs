using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Entity configuration for UserInfo entity
/// </summary>
public class UserInfoConfiguration : IEntityTypeConfiguration<UserInfo>
{
    public void Configure(EntityTypeBuilder<UserInfo> builder)
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

        builder.Property(u => u.Password)
            .HasColumnName("Password")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Dob)
            .HasColumnName("dob")
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
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasMany(u => u.Bookings)
            .WithOne(b => b.User)
            .HasForeignKey(b => b.Email)
            .HasPrincipalKey(u => u.Email)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace Tests.TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Unit tests for BookingConfiguration
/// </summary>
public class BookingConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public BookingConfigurationTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_SetsTableName()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("booking", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        var primaryKey = entityType!.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_SetsEmailAsRequired()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var emailProperty = entityType!.FindProperty(nameof(Booking.Email));

        // Assert
        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsEmailMaxLength()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var emailProperty = entityType!.FindProperty(nameof(Booking.Email));

        // Assert
        Assert.NotNull(emailProperty);
        Assert.Equal(200, emailProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsIsActiveDefaultValue()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var isActiveProperty = entityType!.FindProperty(nameof(Booking.IsActive));

        // Assert
        Assert.NotNull(isActiveProperty);
        Assert.Equal(true, isActiveProperty.GetDefaultValue());
    }

    [Fact]
    public void Configure_ConfiguresTourRelationship()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var navigation = entityType!.FindNavigation(nameof(Booking.Tour));

        // Assert
        Assert.NotNull(navigation);
        Assert.False(navigation.IsCollection);
        Assert.Equal(typeof(Tour), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public void Configure_ConfiguresUserRelationship()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var navigation = entityType!.FindNavigation(nameof(Booking.User));

        // Assert
        Assert.NotNull(navigation);
        Assert.False(navigation.IsCollection);
        Assert.Equal(typeof(User), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public async Task CanAddBookingWithConfiguration()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Email = "test@example.com",
            FirstName = "John",
            IsActive = true
        };

        // Act
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        // Assert
        var savedBooking = await context.Bookings.FirstOrDefaultAsync();
        Assert.NotNull(savedBooking);
        Assert.Equal("test@example.com", savedBooking.Email);
    }
}

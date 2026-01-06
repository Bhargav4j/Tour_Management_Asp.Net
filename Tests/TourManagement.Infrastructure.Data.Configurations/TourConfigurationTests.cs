using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace Tests.TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Unit tests for TourConfiguration
/// </summary>
public class TourConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourConfigurationTests()
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
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Tour", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Assert
        var primaryKey = entityType!.FindPrimaryKey();
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_SetsNameAsRequired()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var nameProperty = entityType!.FindProperty(nameof(Tour.Name));

        // Assert
        Assert.NotNull(nameProperty);
        Assert.False(nameProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsNameMaxLength()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var nameProperty = entityType!.FindProperty(nameof(Tour.Name));

        // Assert
        Assert.NotNull(nameProperty);
        Assert.Equal(200, nameProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsPriceColumnType()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var priceProperty = entityType!.FindProperty(nameof(Tour.Price));

        // Assert
        Assert.NotNull(priceProperty);
        Assert.Equal("decimal(18,2)", priceProperty.GetColumnType());
    }

    [Fact]
    public void Configure_SetsIsActiveDefaultValue()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var isActiveProperty = entityType!.FindProperty(nameof(Tour.IsActive));

        // Assert
        Assert.NotNull(isActiveProperty);
        Assert.Equal(true, isActiveProperty.GetDefaultValue());
    }

    [Fact]
    public void Configure_ConfiguresBookingsRelationship()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var navigation = entityType!.FindNavigation(nameof(Tour.Bookings));

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
        Assert.Equal(typeof(Booking), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public async Task CanAddTourWithConfiguration()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour
        {
            Name = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000m,
            Locations = "Location 1, Location 2",
            IsActive = true
        };

        // Act
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        // Assert
        var savedTour = await context.Tours.FirstOrDefaultAsync();
        Assert.NotNull(savedTour);
        Assert.Equal("Test Tour", savedTour.Name);
    }
}

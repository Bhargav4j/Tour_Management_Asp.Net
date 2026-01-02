using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using Xunit;

namespace TourManagement.UnitTests.Configuration;

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
    public void Configure_ShouldSetTableName()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act & Assert
        Assert.Equal("Tour", entityType!.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_ShouldSetTourNameAsRequired()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var property = entityType!.FindProperty("TourName");

        // Assert
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void Configure_ShouldSetPriceColumnType()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var property = entityType!.FindProperty("Price");

        // Assert
        Assert.Equal("decimal(18,2)", property!.GetColumnType());
    }

    [Fact]
    public void Configure_ShouldSetIsActiveDefaultValue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var property = entityType!.FindProperty("IsActive");

        // Assert
        Assert.NotNull(property!.GetDefaultValue());
        Assert.Equal(true, property.GetDefaultValue());
    }

    [Fact]
    public void Configure_ShouldSetCreatedByDefaultValue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var property = entityType!.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(property!.GetDefaultValue());
        Assert.Equal("System", property.GetDefaultValue());
    }

    [Fact]
    public void Configure_ShouldConfigureBookingsRelationship()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Act
        var navigation = entityType!.FindNavigation("Bookings");

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
    }
}

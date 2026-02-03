using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations.Tests;

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
    public void Configure_ShouldMapTourEntityToTourTable()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Tour", entityType.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetIdAsPrimaryKey()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties[0].Name);
    }

    [Fact]
    public void Configure_ShouldConfigureTourNameAsRequired()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var property = entityType?.FindProperty("TourName");

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.Equal(20, property.GetMaxLength());
    }

    [Fact]
    public void Configure_ShouldConfigurePlaceAsRequired()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var property = entityType?.FindProperty("Place");

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.Equal(20, property.GetMaxLength());
    }

    [Fact]
    public void Configure_ShouldConfigurePriceWithPrecision()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var property = entityType?.FindProperty("Price");

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.Equal(18, property.GetPrecision());
        Assert.Equal(2, property.GetScale());
    }

    [Fact]
    public void Configure_ShouldConfigureIsActiveWithDefaultValue()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var property = entityType?.FindProperty("IsActive");

        // Assert
        Assert.NotNull(property);
        Assert.False(property.IsNullable);
        Assert.True(property.GetDefaultValue() is bool);
    }

    [Fact]
    public void Configure_ShouldConfigurePictureFileNameAsNullable()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var property = entityType?.FindProperty("PictureFileName");

        // Assert
        Assert.NotNull(property);
        Assert.True(property.IsNullable);
    }

    [Fact]
    public void Configure_ShouldConfigureBookingsRelationship()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var navigation = entityType?.FindNavigation("Bookings");

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
    }
}

using Microsoft.EntityFrameworkCore;
using Xunit;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations.Tests;

public class TourConfigurationTests
{
    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Configure_ShouldSetTableName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Tour", entityType.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("TourId", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_TourId_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourIdProperty = entityType?.FindProperty("TourId");

        // Assert
        Assert.NotNull(tourIdProperty);
        Assert.Equal("TOUR_ID", tourIdProperty.GetColumnName());
    }

    [Fact]
    public void Configure_TourName_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourNameProperty = entityType?.FindProperty("TourName");

        // Assert
        Assert.NotNull(tourNameProperty);
        Assert.False(tourNameProperty.IsNullable);
    }

    [Fact]
    public void Configure_TourName_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourNameProperty = entityType?.FindProperty("TourName");

        // Assert
        Assert.NotNull(tourNameProperty);
        Assert.Equal("TOUR_NAME", tourNameProperty.GetColumnName());
    }

    [Fact]
    public void Configure_TourName_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourNameProperty = entityType?.FindProperty("TourName");

        // Assert
        Assert.NotNull(tourNameProperty);
        Assert.Equal(200, tourNameProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Place_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var placeProperty = entityType?.FindProperty("Place");

        // Assert
        Assert.NotNull(placeProperty);
        Assert.False(placeProperty.IsNullable);
    }

    [Fact]
    public void Configure_Place_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var placeProperty = entityType?.FindProperty("Place");

        // Assert
        Assert.NotNull(placeProperty);
        Assert.Equal("PLACE", placeProperty.GetColumnName());
    }

    [Fact]
    public void Configure_Place_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var placeProperty = entityType?.FindProperty("Place");

        // Assert
        Assert.NotNull(placeProperty);
        Assert.Equal(200, placeProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Days_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var daysProperty = entityType?.FindProperty("Days");

        // Assert
        Assert.NotNull(daysProperty);
        Assert.False(daysProperty.IsNullable);
    }

    [Fact]
    public void Configure_Days_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var daysProperty = entityType?.FindProperty("Days");

        // Assert
        Assert.NotNull(daysProperty);
        Assert.Equal("DAYS", daysProperty.GetColumnName());
    }

    [Fact]
    public void Configure_Price_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var priceProperty = entityType?.FindProperty("Price");

        // Assert
        Assert.NotNull(priceProperty);
        Assert.False(priceProperty.IsNullable);
    }

    [Fact]
    public void Configure_Price_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var priceProperty = entityType?.FindProperty("Price");

        // Assert
        Assert.NotNull(priceProperty);
        Assert.Equal("PRICE", priceProperty.GetColumnName());
    }

    [Fact]
    public void Configure_Price_ShouldHaveColumnType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var priceProperty = entityType?.FindProperty("Price");

        // Assert
        Assert.NotNull(priceProperty);
        Assert.Equal("decimal(18,2)", priceProperty.GetColumnType());
    }

    [Fact]
    public void Configure_Locations_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var locationsProperty = entityType?.FindProperty("Locations");

        // Assert
        Assert.NotNull(locationsProperty);
        Assert.Equal("LOCATIONS", locationsProperty.GetColumnName());
    }

    [Fact]
    public void Configure_Locations_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var locationsProperty = entityType?.FindProperty("Locations");

        // Assert
        Assert.NotNull(locationsProperty);
        Assert.Equal(500, locationsProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_TourInfo_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourInfoProperty = entityType?.FindProperty("TourInfo");

        // Assert
        Assert.NotNull(tourInfoProperty);
        Assert.Equal("TOUR_INFO", tourInfoProperty.GetColumnName());
    }

    [Fact]
    public void Configure_TourInfo_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var tourInfoProperty = entityType?.FindProperty("TourInfo");

        // Assert
        Assert.NotNull(tourInfoProperty);
        Assert.Equal(2000, tourInfoProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_PictureFileName_ShouldHaveColumnName()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var pictureProperty = entityType?.FindProperty("PictureFileName");

        // Assert
        Assert.NotNull(pictureProperty);
        Assert.Equal("pic", pictureProperty.GetColumnName());
    }

    [Fact]
    public void Configure_PictureFileName_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var pictureProperty = entityType?.FindProperty("PictureFileName");

        // Assert
        Assert.NotNull(pictureProperty);
        Assert.Equal(500, pictureProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_Bookings_ShouldHaveRelationship()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var bookingsNavigation = entityType?.FindNavigation("Bookings");

        // Assert
        Assert.NotNull(bookingsNavigation);
    }
}

using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using Xunit;

namespace TourManagement.UnitTests.Configuration;

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
    public void Configure_ShouldSetTableName()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act & Assert
        Assert.Equal("Booking", entityType!.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_ShouldSetUserIdAsRequired()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var property = entityType!.FindProperty("UserId");

        // Assert
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void Configure_ShouldSetTourIdAsRequired()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var property = entityType!.FindProperty("TourId");

        // Assert
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void Configure_ShouldSetTotalAmountColumnType()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var property = entityType!.FindProperty("TotalAmount");

        // Assert
        Assert.Equal("decimal(18,2)", property!.GetColumnType());
    }

    [Fact]
    public void Configure_ShouldSetStatusDefaultValue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var property = entityType!.FindProperty("Status");

        // Assert
        Assert.NotNull(property!.GetDefaultValue());
        Assert.Equal("Pending", property.GetDefaultValue());
    }

    [Fact]
    public void Configure_ShouldSetIsActiveDefaultValue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

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
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var property = entityType!.FindProperty("CreatedBy");

        // Assert
        Assert.NotNull(property!.GetDefaultValue());
        Assert.Equal("System", property.GetDefaultValue());
    }

    [Fact]
    public void Configure_ShouldConfigureUserRelationship()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var navigation = entityType!.FindNavigation("User");

        // Assert
        Assert.NotNull(navigation);
        Assert.False(navigation.IsCollection);
    }

    [Fact]
    public void Configure_ShouldConfigureTourRelationship()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Act
        var navigation = entityType!.FindNavigation("Tour");

        // Assert
        Assert.NotNull(navigation);
        Assert.False(navigation.IsCollection);
    }
}

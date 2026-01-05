using Microsoft.EntityFrameworkCore;
using Xunit;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Configurations.Tests;

public class BookingConfigurationTests
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
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Booking", entityType.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var primaryKey = entityType?.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("BookingId", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_UserId_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var userIdProperty = entityType?.FindProperty("UserId");

        // Assert
        Assert.NotNull(userIdProperty);
        Assert.False(userIdProperty.IsNullable);
    }

    [Fact]
    public void Configure_TourId_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var tourIdProperty = entityType?.FindProperty("TourId");

        // Assert
        Assert.NotNull(tourIdProperty);
        Assert.False(tourIdProperty.IsNullable);
    }

    [Fact]
    public void Configure_BookingDate_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var bookingDateProperty = entityType?.FindProperty("BookingDate");

        // Assert
        Assert.NotNull(bookingDateProperty);
        Assert.False(bookingDateProperty.IsNullable);
    }

    [Fact]
    public void Configure_NumberOfPeople_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var numberOfPeopleProperty = entityType?.FindProperty("NumberOfPeople");

        // Assert
        Assert.NotNull(numberOfPeopleProperty);
        Assert.False(numberOfPeopleProperty.IsNullable);
    }

    [Fact]
    public void Configure_TotalPrice_ShouldBeRequired()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var totalPriceProperty = entityType?.FindProperty("TotalPrice");

        // Assert
        Assert.NotNull(totalPriceProperty);
        Assert.False(totalPriceProperty.IsNullable);
    }

    [Fact]
    public void Configure_TotalPrice_ShouldHaveColumnType()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var totalPriceProperty = entityType?.FindProperty("TotalPrice");

        // Assert
        Assert.NotNull(totalPriceProperty);
        Assert.Equal("decimal(18,2)", totalPriceProperty.GetColumnType());
    }

    [Fact]
    public void Configure_Status_ShouldHaveMaxLength()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var statusProperty = entityType?.FindProperty("Status");

        // Assert
        Assert.NotNull(statusProperty);
        Assert.Equal(50, statusProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_User_ShouldHaveRelationship()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var userNavigation = entityType?.FindNavigation("User");

        // Assert
        Assert.NotNull(userNavigation);
    }

    [Fact]
    public void Configure_Tour_ShouldHaveRelationship()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var tourNavigation = entityType?.FindNavigation("Tour");

        // Assert
        Assert.NotNull(tourNavigation);
    }

    [Fact]
    public void Configure_UserForeignKey_ShouldExist()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var foreignKeys = entityType?.GetForeignKeys();
        var userForeignKey = foreignKeys?.FirstOrDefault(fk => fk.Properties.Any(p => p.Name == "UserId"));

        // Assert
        Assert.NotNull(userForeignKey);
    }

    [Fact]
    public void Configure_TourForeignKey_ShouldExist()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var foreignKeys = entityType?.GetForeignKeys();
        var tourForeignKey = foreignKeys?.FirstOrDefault(fk => fk.Properties.Any(p => p.Name == "TourId"));

        // Assert
        Assert.NotNull(tourForeignKey);
    }
}

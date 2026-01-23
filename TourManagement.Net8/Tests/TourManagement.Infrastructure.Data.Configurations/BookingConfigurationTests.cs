using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;
using System;

namespace TourManagement.Infrastructure.Data.Configurations.Tests;

public class BookingConfigurationTests
{
    [Fact]
    public void Configure_SetsCorrectTableName()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new TourManagementDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Booking", entityType.GetTableName());
    }

    [Fact]
    public void Configure_HasPrimaryKey()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new TourManagementDbContext(options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        // Assert
        Assert.NotNull(entityType);
        var primaryKey = entityType.FindPrimaryKey();
        Assert.NotNull(primaryKey);
    }
}

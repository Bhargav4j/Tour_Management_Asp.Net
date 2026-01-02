using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using Xunit;

namespace TourManagement.UnitTests.Configuration;

public class UserConfigurationTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public UserConfigurationTests()
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
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act & Assert
        Assert.Equal("UserInfo", entityType!.GetTableName());
    }

    [Fact]
    public void Configure_ShouldSetPrimaryKey()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act
        var primaryKey = entityType!.FindPrimaryKey();

        // Assert
        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public void Configure_ShouldSetEmailAsRequired()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act
        var property = entityType!.FindProperty("Email");

        // Assert
        Assert.False(property!.IsNullable);
    }

    [Fact]
    public void Configure_ShouldSetEmailAsUniqueIndex()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act
        var indexes = entityType!.GetIndexes();
        var emailIndex = indexes.FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));

        // Assert
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public void Configure_ShouldSetIsActiveDefaultValue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

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
        var entityType = context.Model.FindEntityType(typeof(User));

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
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act
        var navigation = entityType!.FindNavigation("Bookings");

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
    }

    [Fact]
    public void Configure_ShouldMapPasswordHashToPasswordColumn()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        // Act
        var property = entityType!.FindProperty("PasswordHash");

        // Assert
        Assert.Equal("Password", property!.GetColumnName());
    }
}

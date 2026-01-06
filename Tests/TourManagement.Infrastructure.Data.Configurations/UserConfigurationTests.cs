using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Data.Configurations;
using TourManagement.Domain.Entities;

namespace Tests.TourManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Unit tests for UserConfiguration
/// </summary>
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
    public void Configure_SetsTableName()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        // Assert
        Assert.NotNull(entityType);
        Assert.Equal("Userinfo", entityType.GetTableName());
    }

    [Fact]
    public void Configure_SetsPrimaryKey()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

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
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType!.FindProperty(nameof(User.Email));

        // Assert
        Assert.NotNull(emailProperty);
        Assert.False(emailProperty.IsNullable);
    }

    [Fact]
    public void Configure_SetsEmailMaxLength()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType!.FindProperty(nameof(User.Email));

        // Assert
        Assert.NotNull(emailProperty);
        Assert.Equal(200, emailProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsEmailAsUnique()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var emailProperty = entityType!.FindProperty(nameof(User.Email));
        var indexes = entityType.GetIndexes().Where(i => i.Properties.Any(p => p.Name == nameof(User.Email)));

        // Assert
        Assert.NotEmpty(indexes);
        Assert.True(indexes.First().IsUnique);
    }

    [Fact]
    public void Configure_SetsPasswordHashMaxLength()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var passwordHashProperty = entityType!.FindProperty(nameof(User.PasswordHash));

        // Assert
        Assert.NotNull(passwordHashProperty);
        Assert.Equal(500, passwordHashProperty.GetMaxLength());
    }

    [Fact]
    public void Configure_SetsIsActiveDefaultValue()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var isActiveProperty = entityType!.FindProperty(nameof(User.IsActive));

        // Assert
        Assert.NotNull(isActiveProperty);
        Assert.Equal(true, isActiveProperty.GetDefaultValue());
    }

    [Fact]
    public void Configure_ConfiguresBookingsRelationship()
    {
        // Arrange & Act
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var navigation = entityType!.FindNavigation(nameof(User.Bookings));

        // Assert
        Assert.NotNull(navigation);
        Assert.True(navigation.IsCollection);
        Assert.Equal(typeof(Booking), navigation.TargetEntityType.ClrType);
    }

    [Fact]
    public async Task CanAddUserWithConfiguration()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var user = new User
        {
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            FirstName = "John",
            LastName = "Doe",
            IsActive = true
        };

        // Act
        context.Users.Add(user);
        await context.SaveChangesAsync();

        // Assert
        var savedUser = await context.Users.FirstOrDefaultAsync();
        Assert.NotNull(savedUser);
        Assert.Equal("test@example.com", savedUser.Email);
    }
}

using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Data.Configurations;

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
    public void UserConfiguration_AppliesCorrectly()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));

        Assert.NotNull(entityType);
        Assert.Equal("UserInfo", entityType.GetTableName());
    }

    [Fact]
    public void UserConfiguration_HasCorrectPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(User));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Email", primaryKey.Properties.First().Name);
    }

    [Fact]
    public async Task UserConfiguration_CanSaveUser()
    {
        using var context = new TourManagementDbContext(_options);
        var user = new User
        {
            Email = "config@test.com",
            FirstName = "Config",
            LastName = "Test",
            Gender = "Male",
            PasswordHash = "hash",
            DateOfBirth = DateTime.UtcNow,
            Street = "Street",
            City = "City",
            State = "State"
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var saved = await context.Users.FindAsync("config@test.com");
        Assert.NotNull(saved);
        Assert.Equal("Config", saved.FirstName);
    }
}

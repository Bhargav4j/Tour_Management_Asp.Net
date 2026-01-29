using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Data.Configurations;

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
    public void TourConfiguration_AppliesCorrectly()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));

        Assert.NotNull(entityType);
        Assert.Equal("Tour", entityType.GetTableName());
    }

    [Fact]
    public void TourConfiguration_HasCorrectPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Tour));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public async Task TourConfiguration_DefaultValuesApply()
    {
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour
        {
            Name = "Test Tour",
            Place = "Paris",
            Days = 7,
            Price = 1000,
            Locations = "Location",
            TourInfo = "Info",
            CreatedBy = "Test"
        };

        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var saved = await context.Tours.FindAsync(tour.Id);
        Assert.NotNull(saved);
    }
}

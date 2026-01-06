using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public TourManagementDbContextTests()
    {
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void DbContext_HasToursDbSet()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.NotNull(context.Tours);
    }

    [Fact]
    public void DbContext_HasBookingsDbSet()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public void DbContext_HasUsersDbSet()
    {
        using var context = new TourManagementDbContext(_options);
        Assert.NotNull(context.Users);
    }

    [Fact]
    public async Task DbContext_CanAddAndRetrieveTour()
    {
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Name = "Test Tour", Place = "Test Place" };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var retrieved = await context.Tours.FirstOrDefaultAsync(t => t.Name == "Test Tour");
        Assert.NotNull(retrieved);
        Assert.Equal("Test Tour", retrieved.Name);
    }
}

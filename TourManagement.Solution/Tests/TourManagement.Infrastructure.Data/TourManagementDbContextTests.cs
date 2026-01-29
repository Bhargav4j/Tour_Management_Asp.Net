using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Data;

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
    public void Constructor_WithValidOptions_CreatesInstance()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context);
    }

    [Fact]
    public void DbSet_Tours_IsInitialized()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context.Tours);
    }

    [Fact]
    public void DbSet_Users_IsInitialized()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context.Users);
    }

    [Fact]
    public void DbSet_Bookings_IsInitialized()
    {
        using var context = new TourManagementDbContext(_options);

        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task CanAddAndRetrieveTour()
    {
        using var context = new TourManagementDbContext(_options);
        var tour = new Tour { Id = 1, Name = "Test Tour", Place = "Paris", IsActive = true };

        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var retrieved = await context.Tours.FindAsync(1);
        Assert.NotNull(retrieved);
        Assert.Equal("Test Tour", retrieved.Name);
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        using var context = new TourManagementDbContext(_options);
        var user = new User { Email = "test@example.com", FirstName = "Test", IsActive = true };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var retrieved = await context.Users.FindAsync("test@example.com");
        Assert.NotNull(retrieved);
        Assert.Equal("Test", retrieved.FirstName);
    }

    [Fact]
    public async Task CanAddAndRetrieveBooking()
    {
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourId = 1, UserEmail = "user@test.com", IsActive = true };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var retrieved = await context.Bookings.FindAsync(1);
        Assert.NotNull(retrieved);
        Assert.Equal("user@test.com", retrieved.UserEmail);
    }
}

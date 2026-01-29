using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Infrastructure.Data.Configurations;

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
    public void BookingConfiguration_AppliesCorrectly()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));

        Assert.NotNull(entityType);
        Assert.Equal("booking", entityType.GetTableName());
    }

    [Fact]
    public void BookingConfiguration_HasCorrectPrimaryKey()
    {
        using var context = new TourManagementDbContext(_options);
        var entityType = context.Model.FindEntityType(typeof(Booking));
        var primaryKey = entityType!.FindPrimaryKey();

        Assert.NotNull(primaryKey);
        Assert.Single(primaryKey.Properties);
        Assert.Equal("Id", primaryKey.Properties.First().Name);
    }

    [Fact]
    public async Task BookingConfiguration_CanSaveBooking()
    {
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking
        {
            TourId = 1,
            TourName = "Test",
            Place = "Paris",
            UserEmail = "user@test.com",
            FirstName = "User",
            CreatedBy = "Test"
        };

        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var saved = await context.Bookings.FindAsync(booking.Id);
        Assert.NotNull(saved);
        Assert.Equal("Test", saved.TourName);
    }
}

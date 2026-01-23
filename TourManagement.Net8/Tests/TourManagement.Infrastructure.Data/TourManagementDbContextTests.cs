using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    [Fact]
    public void Constructor_WithValidOptions_CreatesContext()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
        Assert.NotNull(context.Users);
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public async Task CanAddAndRetrieveUser()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var user = new User
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Gender = "Male",
                PasswordHash = "hash",
                DateOfBirth = DateTime.Now,
                Street = "Street",
                City = "City",
                State = "State",
                IsActive = true
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var user = await context.Users.FirstOrDefaultAsync();
            Assert.NotNull(user);
            Assert.Equal("test@test.com", user.Email);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveTour()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var tour = new Tour
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 5,
                Price = 1000,
                Locations = "Eiffel Tower, Louvre",
                TourInfo = "Best of Paris",
                IsActive = true
            };
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var tour = await context.Tours.FirstOrDefaultAsync();
            Assert.NotNull(tour);
            Assert.Equal("Paris Tour", tour.TourName);
        }
    }

    [Fact]
    public async Task CanAddAndRetrieveBooking()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var user = new User
            {
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Gender = "Male",
                PasswordHash = "hash",
                DateOfBirth = DateTime.Now,
                Street = "Street",
                City = "City",
                State = "State",
                IsActive = true
            };
            var tour = new Tour
            {
                TourName = "Paris Tour",
                Place = "Paris",
                Days = 5,
                Price = 1000,
                Locations = "Locations",
                TourInfo = "Info",
                IsActive = true
            };
            context.Users.Add(user);
            context.Tours.Add(tour);
            await context.SaveChangesAsync();

            var booking = new Booking
            {
                UserId = user.Id,
                TourId = tour.Id,
                BookingDate = DateTime.Now,
                NumberOfPersons = 2,
                TotalAmount = 2000,
                IsActive = true
            };
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var booking = await context.Bookings
                .Include(b => b.User)
                .Include(b => b.Tour)
                .FirstOrDefaultAsync();
            Assert.NotNull(booking);
            Assert.Equal(2, booking.NumberOfPersons);
        }
    }
}

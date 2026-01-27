using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class TourManagementDbContextTests
{
    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Tours_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new TourManagementDbContext(options);
        var tours = context.Tours;

        // Assert
        Assert.NotNull(tours);
        Assert.IsAssignableFrom<DbSet<Tour>>(tours);
    }

    [Fact]
    public void Users_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new TourManagementDbContext(options);
        var users = context.Users;

        // Assert
        Assert.NotNull(users);
        Assert.IsAssignableFrom<DbSet<User>>(users);
    }

    [Fact]
    public void Bookings_Property_ShouldReturnDbSet()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new TourManagementDbContext(options);
        var bookings = context.Bookings;

        // Assert
        Assert.NotNull(bookings);
        Assert.IsAssignableFrom<DbSet<Booking>>(bookings);
    }

    [Fact]
    public async Task Tours_AddTour_ShouldPersistTour()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Paris",
            Days = 5,
            Price = 1500m
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedTour = await context.Tours.FirstOrDefaultAsync();
            Assert.NotNull(savedTour);
            Assert.Equal("Test Tour", savedTour.TourName);
            Assert.Equal("Paris", savedTour.Place);
        }
    }

    [Fact]
    public async Task Users_AddUser_ShouldPersistUser()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var user = new User
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe"
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedUser = await context.Users.FirstOrDefaultAsync();
            Assert.NotNull(savedUser);
            Assert.Equal("test@example.com", savedUser.Email);
            Assert.Equal("John", savedUser.FirstName);
        }
    }

    [Fact]
    public async Task Bookings_AddBooking_ShouldPersistBooking()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var booking = new Booking
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            TotalAmount = 3000m,
            Status = "Confirmed"
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Bookings.Add(booking);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedBooking = await context.Bookings.FirstOrDefaultAsync();
            Assert.NotNull(savedBooking);
            Assert.Equal(1, savedBooking.UserId);
            Assert.Equal(1, savedBooking.TourId);
            Assert.Equal(2, savedBooking.NumberOfPeople);
        }
    }

    [Fact]
    public async Task Tours_UpdateTour_ShouldPersistChanges()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour { TourName = "Original Name", Place = "Rome" };

        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var tourToUpdate = await context.Tours.FirstAsync();
            tourToUpdate.TourName = "Updated Name";
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var updatedTour = await context.Tours.FirstAsync();
            Assert.Equal("Updated Name", updatedTour.TourName);
        }
    }

    [Fact]
    public async Task Tours_DeleteTour_ShouldRemoveTour()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour { TourName = "To Delete", Place = "London" };

        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var tourToDelete = await context.Tours.FirstAsync();
            context.Tours.Remove(tourToDelete);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var count = await context.Tours.CountAsync();
            Assert.Equal(0, count);
        }
    }

    [Fact]
    public async Task Context_AddMultipleTours_ShouldPersistAll()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tours = new List<Tour>
        {
            new Tour { TourName = "Tour 1", Place = "Paris" },
            new Tour { TourName = "Tour 2", Place = "Rome" },
            new Tour { TourName = "Tour 3", Place = "Berlin" }
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.AddRange(tours);
            await context.SaveChangesAsync();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var count = await context.Tours.CountAsync();
            Assert.Equal(3, count);
        }
    }

    [Fact]
    public async Task Context_QueryTours_ShouldReturnCorrectResults()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tours = new List<Tour>
        {
            new Tour { TourName = "Paris Tour", Place = "Paris", Price = 1000m },
            new Tour { TourName = "Rome Tour", Place = "Rome", Price = 1500m }
        };

        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.AddRange(tours);
            await context.SaveChangesAsync();
        }

        // Act & Assert
        using (var context = new TourManagementDbContext(options))
        {
            var parisTour = await context.Tours
                .Where(t => t.Place == "Paris")
                .FirstOrDefaultAsync();

            Assert.NotNull(parisTour);
            Assert.Equal("Paris Tour", parisTour.TourName);
        }
    }

    [Fact]
    public void DbContext_Model_ShouldIncludeAllEntityTypes()
    {
        // Arrange
        var options = CreateInMemoryOptions();

        // Act
        using var context = new TourManagementDbContext(options);
        var model = context.Model;

        // Assert
        var entityTypes = model.GetEntityTypes().Select(e => e.ClrType).ToList();
        Assert.Contains(typeof(Tour), entityTypes);
        Assert.Contains(typeof(User), entityTypes);
        Assert.Contains(typeof(Booking), entityTypes);
    }

    [Fact]
    public async Task Context_SaveChangesAsync_ShouldReturnNumberOfAffectedEntries()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour { TourName = "Test Tour", Place = "Test" };

        // Act
        using var context = new TourManagementDbContext(options);
        context.Tours.Add(tour);
        var result = await context.SaveChangesAsync();

        // Assert
        Assert.Equal(1, result);
    }

    [Fact]
    public async Task Context_DisposeAfterUse_ShouldNotThrowException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var context = new TourManagementDbContext(options);

        // Act
        context.Tours.Add(new Tour { TourName = "Test", Place = "Test" });
        await context.SaveChangesAsync();
        context.Dispose();

        // Assert - No exception should be thrown
        Assert.True(true);
    }
}

using Xunit;
using Microsoft.EntityFrameworkCore;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Data.Tests;

public class TourManagementDbContextTests
{
    private DbContextOptions<TourManagementDbContext> CreateInMemoryDbOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidOptions_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context);
    }

    [Fact]
    public void Tours_DbSet_ShouldBeInitialized()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context.Tours);
    }

    [Fact]
    public void Users_DbSet_ShouldBeInitialized()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context.Users);
    }

    [Fact]
    public void Bookings_DbSet_ShouldBeInitialized()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();

        // Act
        using var context = new TourManagementDbContext(options);

        // Assert
        Assert.NotNull(context.Bookings);
    }

    [Fact]
    public void Tours_CanAddAndRetrieveEntity()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        var tour = new Tour
        {
            TourId = 1,
            TourName = "Paris Tour",
            Place = "Paris",
            Days = 5,
            Price = 1500
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var retrievedTour = context.Tours.FirstOrDefault(t => t.TourId == 1);
            Assert.NotNull(retrievedTour);
            Assert.Equal("Paris Tour", retrievedTour.TourName);
        }
    }

    [Fact]
    public void Users_CanAddAndRetrieveEntity()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
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
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var retrievedUser = context.Users.FirstOrDefault(u => u.Email == "test@example.com");
            Assert.NotNull(retrievedUser);
            Assert.Equal("John", retrievedUser.FirstName);
        }
    }

    [Fact]
    public void Bookings_CanAddAndRetrieveEntity()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        var booking = new Booking
        {
            BookingId = 1,
            TourId = 1,
            Email = "test@example.com",
            BookingDate = DateTime.UtcNow
        };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Bookings.Add(booking);
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var retrievedBooking = context.Bookings.FirstOrDefault(b => b.BookingId == 1);
            Assert.NotNull(retrievedBooking);
            Assert.Equal(1, retrievedBooking.TourId);
        }
    }

    [Fact]
    public void Context_CanHandleMultipleTours()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        var tour1 = new Tour { TourId = 1, TourName = "Tour 1" };
        var tour2 = new Tour { TourId = 2, TourName = "Tour 2" };

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour1);
            context.Tours.Add(tour2);
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            Assert.Equal(2, context.Tours.Count());
        }
    }

    [Fact]
    public void Context_CanUpdateEntity()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        var tour = new Tour { TourId = 1, TourName = "Original Name" };

        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            context.SaveChanges();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var tourToUpdate = context.Tours.First();
            tourToUpdate.TourName = "Updated Name";
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var updatedTour = context.Tours.First();
            Assert.Equal("Updated Name", updatedTour.TourName);
        }
    }

    [Fact]
    public void Context_CanDeleteEntity()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        var tour = new Tour { TourId = 1, TourName = "Tour to Delete" };

        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(tour);
            context.SaveChanges();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var tourToDelete = context.Tours.First();
            context.Tours.Remove(tourToDelete);
            context.SaveChanges();
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            Assert.Empty(context.Tours);
        }
    }

    [Fact]
    public void Context_SaveChanges_ShouldPersistData()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();

        // Act
        int savedEntities;
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(new Tour { TourId = 1, TourName = "Test Tour" });
            savedEntities = context.SaveChanges();
        }

        // Assert
        Assert.Equal(1, savedEntities);
    }

    [Fact]
    public void Context_CanQueryWithLinq()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        using (var context = new TourManagementDbContext(options))
        {
            context.Tours.Add(new Tour { TourId = 1, TourName = "Paris Tour", Price = 1000 });
            context.Tours.Add(new Tour { TourId = 2, TourName = "Rome Tour", Price = 2000 });
            context.SaveChanges();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var expensiveTours = context.Tours.Where(t => t.Price > 1500).ToList();

            // Assert
            Assert.Single(expensiveTours);
            Assert.Equal("Rome Tour", expensiveTours[0].TourName);
        }
    }

    [Fact]
    public void Context_AllDbSets_ShouldBeAccessible()
    {
        // Arrange
        var options = CreateInMemoryDbOptions();
        using var context = new TourManagementDbContext(options);

        // Act & Assert
        Assert.NotNull(context.Tours);
        Assert.NotNull(context.Users);
        Assert.NotNull(context.Bookings);
        Assert.IsAssignableFrom<DbSet<Tour>>(context.Tours);
        Assert.IsAssignableFrom<DbSet<User>>(context.Users);
        Assert.IsAssignableFrom<DbSet<Booking>>(context.Bookings);
    }
}

using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests.Repositories;

public class TourRepositoryTests
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<TourRepository>> _mockLogger;
    private readonly TourRepository _tourRepository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<TourRepository>>();
        _tourRepository = new TourRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new TourRepository(_context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", IsActive = true },
            new Tour { Id = 2, TourName = "Rome Tour", Place = "Rome", IsActive = true },
            new Tour { Id = 3, TourName = "London Tour", Place = "London", IsActive = false }
        };
        _context.Tours.AddRange(tours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingActiveTour_ShouldReturnTour()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = false };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingTour_ShouldReturnNull()
    {
        // Act
        var result = await _tourRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTourToDatabase()
    {
        // Arrange
        var tour = new Tour
        {
            TourName = "New Tour",
            Place = "New Place",
            Days = 5,
            Price = 1000m,
            IsActive = true
        };

        // Act
        var result = await _tourRepository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedTour = await _context.Tours.FindAsync(result.Id);
        Assert.NotNull(savedTour);
        Assert.Equal("New Tour", savedTour.TourName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateTourInDatabase()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Old Tour", Place = "Old Place", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();
        _context.Entry(tour).State = EntityState.Detached;

        tour.TourName = "Updated Tour";

        // Act
        await _tourRepository.UpdateAsync(tour);

        // Assert
        var updatedTour = await _context.Tours.FindAsync(1);
        Assert.NotNull(updatedTour);
        Assert.Equal("Updated Tour", updatedTour.TourName);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetTourAsInactive()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        await _tourRepository.DeleteAsync(1);

        // Assert
        var deletedTour = await _context.Tours.FindAsync(1);
        Assert.NotNull(deletedTour);
        Assert.False(deletedTour.IsActive);
        Assert.NotNull(deletedTour.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingTour_ShouldNotThrow()
    {
        // Act & Assert
        await _tourRepository.DeleteAsync(999);
        // Should not throw exception
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ShouldReturnTrue()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = false };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingTour_ShouldReturnFalse()
    {
        // Act
        var result = await _tourRepository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Locations = "Eiffel Tower", IsActive = true },
            new Tour { Id = 2, TourName = "Paris Adventure", Place = "Paris", Locations = "Louvre", IsActive = true },
            new Tour { Id = 3, TourName = "Rome Tour", Place = "Rome", Locations = "Colosseum", IsActive = true }
        };
        _context.Tours.AddRange(tours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count()); // Matches TourName and Place
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1", Place = "Paris", Locations = "Location 1", IsActive = true },
            new Tour { Id = 2, TourName = "Tour 2", Place = "Paris", Locations = "Location 2", IsActive = true }
        };
        _context.Tours.AddRange(tours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLocations_ShouldReturnTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour 1", Place = "Paris", Locations = "Eiffel Tower", IsActive = true },
            new Tour { Id = 2, TourName = "Tour 2", Place = "Rome", Locations = "Eiffel Tower replica", IsActive = true }
        };
        _context.Tours.AddRange(tours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.SearchAsync("Eiffel");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyCollection()
    {
        // Arrange
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", Locations = "Test Location", IsActive = true };
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.SearchAsync("xyz");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldExcludeInactiveTours()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Paris Tour", Place = "Paris", Locations = "Eiffel", IsActive = true },
            new Tour { Id = 2, TourName = "Paris Adventure", Place = "Paris", Locations = "Louvre", IsActive = false }
        };
        _context.Tours.AddRange(tours);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.SearchAsync("Paris");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, t => Assert.True(t.IsActive));
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeBookings()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test Place", IsActive = true };
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true };

        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _tourRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Bookings);
        Assert.Single(result.Bookings);
    }
}

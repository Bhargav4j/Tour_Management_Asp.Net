using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests.Repositories;

public class BookingRepositoryTests
{
    private readonly TourManagementDbContext _context;
    private readonly Mock<ILogger<BookingRepository>> _mockLogger;
    private readonly BookingRepository _bookingRepository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        _mockLogger = new Mock<ILogger<BookingRepository>>();
        _bookingRepository = new BookingRepository(_context, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingRepository(_context, null!));
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnOnlyActiveBookings()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true },
            new Booking { Id = 3, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = false }
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingActiveBooking_ShouldReturnBooking()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.NotNull(result.User);
        Assert.NotNull(result.Tour);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ShouldReturnNull()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = false };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingBooking_ShouldReturnNull()
    {
        // Act
        var result = await _bookingRepository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var user1 = new User { Id = 1, Email = "user1@test.com", FirstName = "User1", IsActive = true };
        var user2 = new User { Id = 2, Email = "user2@test.com", FirstName = "User2", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, User = user1, Tour = tour, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 1, User = user1, Tour = tour, IsActive = true },
            new Booking { Id = 3, UserId = 2, TourId = 1, User = user2, Tour = tour, IsActive = true }
        };
        _context.Users.AddRange(user1, user2);
        _context.Tours.Add(tour);
        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
        Assert.All(result, b => Assert.NotNull(b.Tour));
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldExcludeInactiveBookings()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = false }
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldReturnTourBookings()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour1 = new Tour { Id = 1, TourName = "Tour 1", Place = "Test", IsActive = true };
        var tour2 = new Tour { Id = 2, TourName = "Tour 2", Place = "Test", IsActive = true };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour1, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 1, User = user, Tour = tour1, IsActive = true },
            new Booking { Id = 3, UserId = 1, TourId = 2, User = user, Tour = tour2, IsActive = true }
        };
        _context.Users.Add(user);
        _context.Tours.AddRange(tour1, tour2);
        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
        Assert.All(result, b => Assert.NotNull(b.User));
    }

    [Fact]
    public async Task GetByTourIdAsync_ShouldExcludeInactiveBookings()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = true },
            new Booking { Id = 2, UserId = 1, TourId = 1, User = user, Tour = tour, IsActive = false }
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.AddRange(bookings);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.All(result, b => Assert.True(b.IsActive));
    }

    [Fact]
    public async Task AddAsync_ShouldAddBookingToDatabase()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        await _context.SaveChangesAsync();

        var booking = new Booking
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            TotalAmount = 1000m,
            Status = "Confirmed",
            IsActive = true
        };

        // Act
        var result = await _bookingRepository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        var savedBooking = await _context.Bookings.FindAsync(result.Id);
        Assert.NotNull(savedBooking);
        Assert.Equal(2, savedBooking.NumberOfPeople);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBookingInDatabase()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            User = user,
            Tour = tour,
            NumberOfPeople = 2,
            TotalAmount = 1000m,
            Status = "Pending",
            IsActive = true
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        _context.Entry(booking).State = EntityState.Detached;

        booking.Status = "Confirmed";
        booking.NumberOfPeople = 3;

        // Act
        await _bookingRepository.UpdateAsync(booking);

        // Assert
        var updatedBooking = await _context.Bookings.FindAsync(1);
        Assert.NotNull(updatedBooking);
        Assert.Equal("Confirmed", updatedBooking.Status);
        Assert.Equal(3, updatedBooking.NumberOfPeople);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetBookingAsInactive()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            User = user,
            Tour = tour,
            IsActive = true
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        await _bookingRepository.DeleteAsync(1);

        // Assert
        var deletedBooking = await _context.Bookings.FindAsync(1);
        Assert.NotNull(deletedBooking);
        Assert.False(deletedBooking.IsActive);
        Assert.NotNull(deletedBooking.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithNonExistingBooking_ShouldNotThrow()
    {
        // Act & Assert
        await _bookingRepository.DeleteAsync(999);
        // Should not throw exception
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveBooking_ShouldReturnTrue()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            User = user,
            Tour = tour,
            IsActive = true
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ShouldReturnFalse()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var tour = new Tour { Id = 1, TourName = "Test Tour", Place = "Test", IsActive = true };
        var booking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            User = user,
            Tour = tour,
            IsActive = false
        };
        _context.Users.Add(user);
        _context.Tours.Add(tour);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookingRepository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingBooking_ShouldReturnFalse()
    {
        // Act
        var result = await _bookingRepository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }
}

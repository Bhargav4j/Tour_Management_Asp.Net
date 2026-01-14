using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Infrastructure.Tests;

/// <summary>
/// Test class for BookingRepository
/// </summary>
public class BookingRepositoryTests
{
    private readonly Mock<ILogger<BookingRepository>> _loggerMock;
    private readonly DbContextOptions<TourManagementDbContext> _options;

    public BookingRepositoryTests()
    {
        _loggerMock = new Mock<ILogger<BookingRepository>>();
        _options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidParameters_CreatesInstance()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);

        // Act
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveBookings_ReturnsActiveBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var activeBooking = new Booking { Id = 1, TourName = "Active Booking", IsActive = true };
        var inactiveBooking = new Booking { Id = 2, TourName = "Inactive Booking", IsActive = false };
        context.Bookings.AddRange(activeBooking, inactiveBooking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Active Booking", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoActiveBookings_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Booking", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveBooking_ReturnsNull()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Inactive Booking", IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithValidEmail_ReturnsBookings()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking1 = new Booking { Id = 1, Email = "test@example.com", TourName = "Booking1", IsActive = true };
        var booking2 = new Booking { Id = 2, Email = "test@example.com", TourName = "Booking2", IsActive = true };
        var booking3 = new Booking { Id = 3, Email = "other@example.com", TourName = "Booking3", IsActive = true };
        context.Bookings.AddRange(booking1, booking2, booking3);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal("test@example.com", b.Email));
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("nonexistent@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithInactiveBookings_ReturnsOnlyActive()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var activeBooking = new Booking { Id = 1, Email = "test@example.com", TourName = "Active", IsActive = true };
        var inactiveBooking = new Booking { Id = 2, Email = "test@example.com", TourName = "Inactive", IsActive = false };
        context.Bookings.AddRange(activeBooking, inactiveBooking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.GetByUserEmailAsync("test@example.com");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Active", result.First().TourName);
    }

    [Fact]
    public async Task AddAsync_WithValidBooking_AddsBookingAndReturns()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);
        var booking = new Booking { TourName = "New Booking", Email = "test@example.com", IsActive = true };

        // Act
        var result = await repository.AddAsync(booking);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Booking", result.TourName);
        Assert.True(result.Id > 0);
    }

    [Fact]
    public async Task AddAsync_WithBooking_SavesChangesToDatabase()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);
        var booking = new Booking { TourName = "Test Booking", IsActive = true };

        // Act
        await repository.AddAsync(booking);
        var savedBooking = await context.Bookings.FindAsync(booking.Id);

        // Assert
        Assert.NotNull(savedBooking);
        Assert.Equal("Test Booking", savedBooking.TourName);
    }

    [Fact]
    public async Task AddAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);
        var booking = new Booking { TourName = "Test Booking" };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.AddAsync(booking, cancellationToken);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task UpdateAsync_WithValidBooking_UpdatesBooking()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Original Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);
        booking.TourName = "Updated Booking";

        // Act
        await repository.UpdateAsync(booking);
        var updatedBooking = await context.Bookings.FindAsync(1);

        // Assert
        Assert.NotNull(updatedBooking);
        Assert.Equal("Updated Booking", updatedBooking.TourName);
    }

    [Fact]
    public async Task UpdateAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Test Booking" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.UpdateAsync(booking, cancellationToken);

        // Assert - No exception thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_SetsIsActiveToFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        await repository.DeleteAsync(1);
        var deletedBooking = await context.Bookings.FindAsync(1);

        // Assert
        Assert.NotNull(deletedBooking);
        Assert.False(deletedBooking.IsActive);
        Assert.NotNull(deletedBooking.ModifiedDate);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_DoesNotThrow()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        // No exception should be thrown
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Test Booking" };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        await repository.DeleteAsync(1, cancellationToken);

        // Assert
        var deletedBooking = await context.Bookings.FindAsync(1);
        Assert.False(deletedBooking!.IsActive);
    }

    [Fact]
    public async Task ExistsAsync_WithExistingId_ReturnsTrue()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Test Booking", IsActive = true };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingId_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveBooking_ReturnsFalse()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var booking = new Booking { Id = 1, TourName = "Inactive Booking", IsActive = false };
        context.Bookings.Add(booking);
        await context.SaveChangesAsync();

        var repository = new BookingRepository(context, _loggerMock.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithCancellationToken_UsesToken()
    {
        // Arrange
        using var context = new TourManagementDbContext(_options);
        var repository = new BookingRepository(context, _loggerMock.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.ExistsAsync(1, cancellationToken);

        // Assert
        Assert.False(result);
    }
}

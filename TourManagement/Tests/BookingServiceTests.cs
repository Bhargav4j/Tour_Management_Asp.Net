using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services.Tests;

/// <summary>
/// Unit tests for BookingService
/// </summary>
public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<BookingService>>();
        _service = new BookingService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public void Constructor_WithNullRepository_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BookingService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullMapper_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BookingService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new BookingService(_mockRepository.Object, _mockMapper.Object, null!));
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, TourName = "Tour 1" },
            new Booking { Id = 2, TourName = "Tour 2" }
        };
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { Id = 1, TourName = "Tour 1" },
            new BookingDto { Id = 2, TourName = "Tour 2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(default)).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBooking()
    {
        // Arrange
        var booking = new Booking { Id = 1, TourName = "Test Tour" };
        var bookingDto = new BookingDto { Id = 1, TourName = "Test Tour" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserEmailAsync_WithValidEmail_ReturnsBookings()
    {
        // Arrange
        var email = "user@test.com";
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, Email = email },
            new Booking { Id = 2, Email = email }
        };
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { Id = 1, Email = email },
            new BookingDto { Id = 2, Email = email }
        };

        _mockRepository.Setup(r => r.GetByUserEmailAsync(email, default)).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserEmailAsync(email);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ReturnsCreatedBooking()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            TourName = "New Booking",
            Place = "Hawaii",
            Email = "test@test.com",
            FirstName = "John",
            TourId = 1
        };
        var createdBooking = new Booking { Id = 1, TourName = "New Booking" };
        var bookingDto = new BookingDto { Id = 1, TourName = "New Booking" };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default)).ReturnsAsync(createdBooking);
        _mockMapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>())).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("New Booking", result.TourName);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SetsDefaultValues()
    {
        // Arrange
        var createDto = new BookingCreateDto { TourName = "Test" };
        Booking capturedBooking = null!;

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), default))
            .Callback<Booking, CancellationToken>((b, ct) => capturedBooking = b)
            .ReturnsAsync((Booking b, CancellationToken ct) => b);
        _mockMapper.Setup(m => m.Map<BookingDto>(It.IsAny<Booking>())).Returns(new BookingDto());

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedBooking);
        Assert.Equal("Pending", capturedBooking.Status);
        Assert.True(capturedBooking.IsActive);
        Assert.Equal("System", capturedBooking.CreatedBy);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesBooking()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { Status = "Confirmed" };
        var existingBooking = new Booking { Id = 1, Status = "Pending" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingBooking);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Booking>(b => b.Status == "Confirmed"), default), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { Status = "Confirmed" };
        _mockRepository.Setup(r => r.GetByIdAsync(999, default)).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_DeletesBooking()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, default)).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, default), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_WithSearchTerm_ReturnsMatchingBookings()
    {
        // Arrange
        var searchTerm = "john";
        var bookings = new List<Booking> { new Booking { Id = 1, FirstName = "John" } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, FirstName = "John" } };

        _mockRepository.Setup(r => r.SearchAsync(searchTerm, default)).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.SearchAsync(searchTerm);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesModifiedDateAndBy()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { Status = "Confirmed" };
        var existingBooking = new Booking { Id = 1, Status = "Pending" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(existingBooking);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), default)).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Booking>(b =>
            b.ModifiedDate != null && b.ModifiedBy == "System"), default), Times.Once);
    }
}

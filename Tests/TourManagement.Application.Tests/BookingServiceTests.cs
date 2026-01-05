using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _bookingRepositoryMock;
    private readonly Mock<ITourRepository> _tourRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<BookingService>> _loggerMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _bookingRepositoryMock = new Mock<IBookingRepository>();
        _tourRepositoryMock = new Mock<ITourRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<BookingService>>();
        _service = new BookingService(
            _bookingRepositoryMock.Object,
            _tourRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Act & Assert
        Assert.NotNull(_service);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, UserId = 1, TourId = 1 },
            new Booking { BookingId = 2, UserId = 2, TourId = 2 }
        };
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { BookingId = 1, UserId = 1, TourId = 1 },
            new BookingDto { BookingId = 2, UserId = 2, TourId = 2 }
        };

        _bookingRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _bookingRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var bookings = new List<Booking>();
        var bookingDtos = new List<BookingDto>();

        _bookingRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnBookingDto()
    {
        // Arrange
        var booking = new Booking { BookingId = 1, UserId = 1, TourId = 1 };
        var bookingDto = new BookingDto { BookingId = 1, UserId = 1, TourId = 1 };

        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mapperMock.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BookingId);
        _bookingRepositoryMock.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, UserId = 1, TourId = 1 },
            new Booking { BookingId = 2, UserId = 1, TourId = 2 }
        };
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { BookingId = 1, UserId = 1, TourId = 1 },
            new BookingDto { BookingId = 2, UserId = 1, TourId = 2 }
        };

        _bookingRepositoryMock.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        _bookingRepositoryMock.Verify(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoBookings_ShouldReturnEmptyList()
    {
        // Arrange
        var bookings = new List<Booking>();
        var bookingDtos = new List<BookingDto>();

        _bookingRepositoryMock.Setup(r => r.GetByUserIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mapperMock.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldReturnBookingDto()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            BookingDate = DateTime.Now
        };
        var tour = new Tour { TourId = 1, Price = 100m };
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 2 };
        var createdBooking = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m };
        var bookingDto = new BookingDto { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalPrice = 200m };

        _tourRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _bookingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);
        _mapperMock.Setup(m => m.Map<BookingDto>(createdBooking)).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.BookingId);
        Assert.Equal(200m, result.TotalPrice);
        _bookingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidTourId_ShouldThrowException()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 1,
            TourId = 999,
            NumberOfPeople = 2,
            BookingDate = DateTime.Now
        };

        _tourRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.CreateAsync(createDto));
    }

    [Fact]
    public async Task CreateAsync_ShouldCalculateTotalPriceCorrectly()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 3,
            BookingDate = DateTime.Now
        };
        var tour = new Tour { TourId = 1, Price = 150.50m };
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 3 };
        var createdBooking = new Booking { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 3, TotalPrice = 451.50m };
        var bookingDto = new BookingDto { BookingId = 1, UserId = 1, TourId = 1, NumberOfPeople = 3, TotalPrice = 451.50m };

        _tourRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _mapperMock.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _bookingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);
        _mapperMock.Setup(m => m.Map<BookingDto>(createdBooking)).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.Equal(451.50m, result.TotalPrice);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateBooking()
    {
        // Arrange
        var updateDto = new BookingUpdateDto
        {
            NumberOfPeople = 5,
            Status = "Confirmed",
            BookingDate = DateTime.Now
        };
        var existingBooking = new Booking
        {
            BookingId = 1,
            UserId = 1,
            TourId = 1,
            NumberOfPeople = 2,
            Status = "Pending"
        };

        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mapperMock.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);
        _bookingRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _bookingRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ShouldThrowException()
    {
        // Arrange
        var updateDto = new BookingUpdateDto
        {
            NumberOfPeople = 5,
            Status = "Confirmed",
            BookingDate = DateTime.Now
        };

        _bookingRepositoryMock.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldCallRepository()
    {
        // Arrange
        _bookingRepositoryMock.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _bookingRepositoryMock.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldStillCallRepository()
    {
        // Arrange
        _bookingRepositoryMock.Setup(r => r.DeleteAsync(999, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(999);

        // Assert
        _bookingRepositoryMock.Verify(r => r.DeleteAsync(999, It.IsAny<CancellationToken>()), Times.Once);
    }
}

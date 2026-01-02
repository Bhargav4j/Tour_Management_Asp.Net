using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

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
    public async Task GetAllAsync_ShouldReturnBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, UserId = 1, TourId = 1 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, UserId = 1, TourId = 1 } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal(1, result.First().UserId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnBookingDto_WhenBookingExists()
    {
        // Arrange
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalAmount = 2000 };
        var bookingDto = new BookingDto { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 2, TotalAmount = 2000 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.NumberOfPeople);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenBookingDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ShouldReturnUserBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1 },
            new Booking { Id = 2, UserId = 1, TourId = 2 }
        };
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { Id = 1, UserId = 1, TourId = 1 },
            new BookingDto { Id = 2, UserId = 1, TourId = 2 }
        };

        _mockRepository.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserIdAsync(1);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBooking_AndReturnBookingDto()
    {
        // Arrange
        var createDto = new BookingCreateDto { UserId = 1, TourId = 1, NumberOfPeople = 3, TotalAmount = 3000 };
        var booking = new Booking { UserId = 1, TourId = 1, NumberOfPeople = 3, TotalAmount = 3000 };
        var createdBooking = new Booking { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 3, TotalAmount = 3000 };
        var bookingDto = new BookingDto { Id = 1, UserId = 1, TourId = 1, NumberOfPeople = 3, TotalAmount = 3000 };

        _mockMapper.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _mockRepository.Setup(r => r.AddAsync(booking, It.IsAny<CancellationToken>())).ReturnsAsync(createdBooking);
        _mockMapper.Setup(m => m.Map<BookingDto>(createdBooking)).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.NumberOfPeople);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBooking_WhenBookingExists()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { NumberOfPeople = 5, TotalAmount = 5000, Status = "Confirmed" };
        var existingBooking = new Booking { Id = 1, NumberOfPeople = 3, TotalAmount = 3000, Status = "Pending" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(existingBooking);
        _mockMapper.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(existingBooking, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenBookingDoesNotExist()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { NumberOfPeople = 5, TotalAmount = 5000 };
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBooking()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(null!, _mockMapper.Object, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenMapperIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockRepository.Object, null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenLoggerIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new BookingService(_mockRepository.Object, _mockMapper.Object, null!));
    }
}

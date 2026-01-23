using Xunit;
using Moq;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Exceptions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TourManagement.Application.Services.Tests;

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
    public async Task GetAllAsync_ReturnsBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, NumberOfPersons = 2 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, NumberOfPersons = 2 } };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsBookingDto_WhenBookingExists()
    {
        // Arrange
        var booking = new Booking { Id = 1, NumberOfPersons = 2 };
        var bookingDto = new BookingDto { Id = 1, NumberOfPersons = 2 };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.NumberOfPersons);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenBookingDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserBookings()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, UserId = 10 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, UserId = 10 } };

        _mockRepository.Setup(r => r.GetByUserIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByUserIdAsync(10);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsTourBookings()
    {
        // Arrange
        var bookings = new List<Booking> { new Booking { Id = 1, TourId = 20 } };
        var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, TourId = 20 } };

        _mockRepository.Setup(r => r.GetByTourIdAsync(20, It.IsAny<CancellationToken>())).ReturnsAsync(bookings);
        _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings)).Returns(bookingDtos);

        // Act
        var result = await _service.GetByTourIdAsync(20);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsBookingDto()
    {
        // Arrange
        var createDto = new BookingCreateDto { UserId = 10, TourId = 20, NumberOfPersons = 2 };
        var booking = new Booking { Id = 1, UserId = 10, TourId = 20, NumberOfPersons = 2 };
        var bookingDto = new BookingDto { Id = 1, UserId = 10, TourId = 20, NumberOfPersons = 2 };

        _mockMapper.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
        _mockRepository.Setup(r => r.AddAsync(booking, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.NumberOfPersons);
    }

    [Fact]
    public async Task DeleteAsync_ThrowsNotFoundException_WhenBookingDoesNotExist()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteAsync(999));
    }

    [Fact]
    public async Task UpdateAsync_ThrowsNotFoundException_WhenBookingDoesNotExist()
    {
        // Arrange
        var updateDto = new BookingUpdateDto();
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateAsync(999, updateDto));
    }
}

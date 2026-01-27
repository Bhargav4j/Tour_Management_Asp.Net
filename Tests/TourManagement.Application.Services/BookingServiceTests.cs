using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _mockRepository;
    private readonly Mock<ILogger<BookingService>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _mockRepository = new Mock<IBookingRepository>();
        _mockLogger = new Mock<ILogger<BookingService>>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);
    }

    [Fact]
    public void BookingService_Constructor_CreatesInstance()
    {
        // Arrange & Act
        var service = new BookingService(_mockRepository.Object, _mapper, _mockLogger.Object);

        // Assert
        Assert.NotNull(service);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedBookingDtos()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Booking { Id = 2, UserId = 2, TourId = 2, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, b => b.Status == "Confirmed");
        Assert.Contains(result, b => b.Status == "Pending");
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithEmptyList_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsBookingDto()
    {
        // Arrange
        var booking = new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Confirmed", result.Status);
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsUserBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Booking { Id = 2, UserId = 1, TourId = 2, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByUserIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.UserId));
        _mockRepository.Verify(r => r.GetByUserIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByTourIdAsync_ReturnsTourBookings()
    {
        // Arrange
        var bookings = new List<Booking>
        {
            new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Confirmed", IsActive = true, CreatedDate = DateTime.UtcNow },
            new Booking { Id = 2, UserId = 2, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow }
        };

        _mockRepository.Setup(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);

        // Act
        var result = await _service.GetByTourIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, b => Assert.Equal(1, b.TourId));
        _mockRepository.Verify(r => r.GetByTourIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CreatesBookingAndReturnsBookingDto()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 1,
            TourId = 1,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 4,
            TotalAmount = 4000m,
            Status = "Confirmed"
        };

        var createdBooking = new Booking
        {
            Id = 1,
            UserId = 1,
            TourId = 1,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 4,
            TotalAmount = 4000m,
            Status = "Confirmed",
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdBooking);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Confirmed", result.Status);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithValidId_UpdatesBooking()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new BookingUpdateDto
        {
            BookingDate = DateTime.UtcNow.AddDays(1),
            NumberOfPeople = 3,
            TotalAmount = 3000m,
            Status = "Confirmed"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        _mockRepository.Verify(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ThrowsInvalidOperationException()
    {
        // Arrange
        var updateDto = new BookingUpdateDto { Status = "Confirmed" };

        _mockRepository.Setup(r => r.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Booking?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateAsync(999, updateDto));
    }

    [Fact]
    public async Task DeleteAsync_CallsRepositoryDelete()
    {
        // Arrange
        _mockRepository.Setup(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _service.DeleteAsync(1);

        // Assert
        _mockRepository.Verify(r => r.DeleteAsync(1, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_PassesTokenToRepository()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        _mockRepository.Setup(r => r.GetAllAsync(cts.Token))
            .ReturnsAsync(new List<Booking>());

        // Act
        await _service.GetAllAsync(cts.Token);

        // Assert
        _mockRepository.Verify(r => r.GetAllAsync(cts.Token), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_MapsPropertiesCorrectly()
    {
        // Arrange
        var createDto = new BookingCreateDto
        {
            UserId = 5,
            TourId = 10,
            BookingDate = DateTime.UtcNow,
            NumberOfPeople = 2,
            TotalAmount = 2000m,
            Status = "Pending"
        };

        Booking capturedBooking = null!;
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((booking, ct) => capturedBooking = booking)
            .ReturnsAsync((Booking booking, CancellationToken ct) => booking);

        // Act
        await _service.CreateAsync(createDto);

        // Assert
        Assert.NotNull(capturedBooking);
        Assert.Equal(5, capturedBooking.UserId);
        Assert.Equal(10, capturedBooking.TourId);
        Assert.Equal(2, capturedBooking.NumberOfPeople);
        Assert.Equal(2000m, capturedBooking.TotalAmount);
    }

    [Fact]
    public async Task UpdateAsync_SetsModifiedDate()
    {
        // Arrange
        var existingBooking = new Booking { Id = 1, UserId = 1, TourId = 1, BookingDate = DateTime.UtcNow, NumberOfPeople = 2, TotalAmount = 2000m, Status = "Pending", IsActive = true, CreatedDate = DateTime.UtcNow };
        var updateDto = new BookingUpdateDto { BookingDate = DateTime.UtcNow, NumberOfPeople = 3, TotalAmount = 3000m, Status = "Confirmed" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingBooking);

        Booking capturedBooking = null!;
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()))
            .Callback<Booking, CancellationToken>((booking, ct) => capturedBooking = booking)
            .Returns(Task.CompletedTask);

        // Act
        await _service.UpdateAsync(1, updateDto);

        // Assert
        Assert.NotNull(capturedBooking);
        Assert.NotNull(capturedBooking.ModifiedDate);
    }

    [Fact]
    public async Task GetByUserIdAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByUserIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetByUserIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByTourIdAsync_WithNoBookings_ReturnsEmptyList()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByTourIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Booking>());

        // Act
        var result = await _service.GetByTourIdAsync(999);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}

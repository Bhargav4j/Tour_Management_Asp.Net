using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests
{
    /// <summary>
    /// Tests for BookingService
    /// </summary>
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<ITourRepository> _mockTourRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<BookingService>> _mockLogger;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockTourRepository = new Mock<ITourRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<BookingService>>();
            _bookingService = new BookingService(
                _mockBookingRepository.Object,
                _mockTourRepository.Object,
                _mockUserRepository.Object,
                _mockMapper.Object,
                _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullBookingRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookingService(null!, _mockTourRepository.Object, _mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookingService(_mockBookingRepository.Object, null!, _mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookingService(_mockBookingRepository.Object, _mockTourRepository.Object, null!, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookingService(_mockBookingRepository.Object, _mockTourRepository.Object, _mockUserRepository.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new BookingService(_mockBookingRepository.Object, _mockTourRepository.Object, _mockUserRepository.Object, _mockMapper.Object, null!));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllBookings()
        {
            // Arrange
            var bookings = new List<Booking> { new Booking { Id = 1 }, new Booking { Id = 2 } };
            var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1 }, new BookingDto { Id = 2 } };

            _mockBookingRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);
            _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings))
                .Returns(bookingDtos);

            // Act
            var result = await _bookingService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<BookingDto>)result).Count);
            _mockBookingRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnBooking()
        {
            // Arrange
            var bookingId = 1;
            var booking = new Booking { Id = bookingId, TourId = 10 };
            var bookingDto = new BookingDto { Id = bookingId, TourId = 10 };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);
            _mockMapper.Setup(m => m.Map<BookingDto>(booking))
                .Returns(bookingDto);

            // Act
            var result = await _bookingService.GetByIdAsync(bookingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookingId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var bookingId = 999;
            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            // Act
            var result = await _bookingService.GetByIdAsync(bookingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnUserBookings()
        {
            // Arrange
            var userId = 1;
            var bookings = new List<Booking> { new Booking { Id = 1, UserId = userId } };
            var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, UserId = userId } };

            _mockBookingRepository.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);
            _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings))
                .Returns(bookingDtos);

            // Act
            var result = await _bookingService.GetByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task GetByTourIdAsync_ShouldReturnTourBookings()
        {
            // Arrange
            var tourId = 1;
            var bookings = new List<Booking> { new Booking { Id = 1, TourId = tourId } };
            var bookingDtos = new List<BookingDto> { new BookingDto { Id = 1, TourId = tourId } };

            _mockBookingRepository.Setup(r => r.GetByTourIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(bookings);
            _mockMapper.Setup(m => m.Map<IEnumerable<BookingDto>>(bookings))
                .Returns(bookingDtos);

            // Act
            var result = await _bookingService.GetByTourIdAsync(tourId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidDto_ShouldReturnCreatedBooking()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 1,
                NumberOfPeople = 2,
                TotalAmount = 500.00m
            };
            var booking = new Booking { Id = 1, TourId = 1, UserId = 1 };
            var bookingDto = new BookingDto { Id = 1, TourId = 1, UserId = 1 };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.ExistsAsync(createDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockMapper.Setup(m => m.Map<Booking>(createDto)).Returns(booking);
            _mockBookingRepository.Setup(r => r.AddAsync(booking, It.IsAny<CancellationToken>()))
                .ReturnsAsync(booking);
            _mockMapper.Setup(m => m.Map<BookingDto>(booking)).Returns(bookingDto);

            // Act
            var result = await _bookingService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentTour_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 999,
                UserId = 1,
                NumberOfPeople = 2,
                TotalAmount = 500.00m
            };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentUser_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 999,
                NumberOfPeople = 2,
                TotalAmount = 500.00m
            };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.ExistsAsync(createDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithZeroNumberOfPeople_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 1,
                NumberOfPeople = 0,
                TotalAmount = 500.00m
            };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.ExistsAsync(createDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithNegativeNumberOfPeople_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 1,
                NumberOfPeople = -1,
                TotalAmount = 500.00m
            };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.ExistsAsync(createDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithZeroTotalAmount_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new BookingCreateDto
            {
                TourId = 1,
                UserId = 1,
                NumberOfPeople = 2,
                TotalAmount = 0
            };

            _mockTourRepository.Setup(r => r.ExistsAsync(createDto.TourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.ExistsAsync(createDto.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.CreateAsync(createDto));
        }

        [Fact]
        public async Task UpdateAsync_WithValidDto_ShouldUpdateBooking()
        {
            // Arrange
            var bookingId = 1;
            var updateDto = new BookingUpdateDto
            {
                NumberOfPeople = 3,
                TotalAmount = 750.00m
            };
            var existingBooking = new Booking { Id = bookingId };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBooking);
            _mockMapper.Setup(m => m.Map(updateDto, existingBooking)).Returns(existingBooking);
            _mockBookingRepository.Setup(r => r.UpdateAsync(existingBooking, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _bookingService.UpdateAsync(bookingId, updateDto);

            // Assert
            _mockBookingRepository.Verify(r => r.UpdateAsync(existingBooking, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var bookingId = 999;
            var updateDto = new BookingUpdateDto { NumberOfPeople = 2, TotalAmount = 500.00m };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Booking?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.UpdateAsync(bookingId, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_WithZeroNumberOfPeople_ShouldThrowBusinessException()
        {
            // Arrange
            var bookingId = 1;
            var updateDto = new BookingUpdateDto { NumberOfPeople = 0, TotalAmount = 500.00m };
            var existingBooking = new Booking { Id = bookingId };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBooking);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.UpdateAsync(bookingId, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_WithZeroTotalAmount_ShouldThrowBusinessException()
        {
            // Arrange
            var bookingId = 1;
            var updateDto = new BookingUpdateDto { NumberOfPeople = 2, TotalAmount = 0 };
            var existingBooking = new Booking { Id = bookingId };

            _mockBookingRepository.Setup(r => r.GetByIdAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingBooking);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _bookingService.UpdateAsync(bookingId, updateDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteBooking()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingRepository.Setup(r => r.ExistsAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockBookingRepository.Setup(r => r.DeleteAsync(bookingId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _bookingService.DeleteAsync(bookingId);

            // Assert
            _mockBookingRepository.Verify(r => r.DeleteAsync(bookingId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var bookingId = 999;
            _mockBookingRepository.Setup(r => r.ExistsAsync(bookingId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _bookingService.DeleteAsync(bookingId));
        }
    }
}

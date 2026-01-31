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
    /// Tests for TourService
    /// </summary>
    public class TourServiceTests
    {
        private readonly Mock<ITourRepository> _mockTourRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<TourService>> _mockLogger;
        private readonly TourService _tourService;

        public TourServiceTests()
        {
            _mockTourRepository = new Mock<ITourRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<TourService>>();
            _tourService = new TourService(_mockTourRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullTourRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TourService(null!, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TourService(_mockTourRepository.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TourService(_mockTourRepository.Object, _mockMapper.Object, null!));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTours()
        {
            // Arrange
            var tours = new List<Tour> { new Tour { Id = 1 }, new Tour { Id = 2 } };
            var tourDtos = new List<TourDto> { new TourDto { Id = 1 }, new TourDto { Id = 2 } };

            _mockTourRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);
            _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
                .Returns(tourDtos);

            // Act
            var result = await _tourService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<TourDto>)result).Count);
            _mockTourRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
        {
            // Arrange
            var tourId = 1;
            var tour = new Tour { Id = tourId, TourName = "Test Tour" };
            var tourDto = new TourDto { Id = tourId, TourName = "Test Tour" };

            _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);
            _mockMapper.Setup(m => m.Map<TourDto>(tour))
                .Returns(tourDto);

            // Act
            var result = await _tourService.GetByIdAsync(tourId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tourId, result.Id);
            Assert.Equal("Test Tour", result.TourName);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var tourId = 999;
            _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tour?)null);

            // Act
            var result = await _tourService.GetByIdAsync(tourId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_WithValidDto_ShouldReturnCreatedTour()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "New Tour",
                Days = 5,
                Price = 1000.00m
            };
            var tour = new Tour { Id = 1, TourName = "New Tour" };
            var tourDto = new TourDto { Id = 1, TourName = "New Tour" };

            _mockMapper.Setup(m => m.Map<Tour>(createDto)).Returns(tour);
            _mockTourRepository.Setup(r => r.AddAsync(tour, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tour);
            _mockMapper.Setup(m => m.Map<TourDto>(tour)).Returns(tourDto);

            // Act
            var result = await _tourService.CreateAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("New Tour", result.TourName);
        }

        [Fact]
        public async Task CreateAsync_WithEmptyTourName_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "",
                Days = 5,
                Price = 1000.00m
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithZeroDays_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "Test Tour",
                Days = 0,
                Price = 1000.00m
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithNegativeDays_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "Test Tour",
                Days = -5,
                Price = 1000.00m
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithZeroPrice_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "Test Tour",
                Days = 5,
                Price = 0
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.CreateAsync(createDto));
        }

        [Fact]
        public async Task CreateAsync_WithNegativePrice_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new TourCreateDto
            {
                TourName = "Test Tour",
                Days = 5,
                Price = -100.00m
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.CreateAsync(createDto));
        }

        [Fact]
        public async Task UpdateAsync_WithValidDto_ShouldUpdateTour()
        {
            // Arrange
            var tourId = 1;
            var updateDto = new TourUpdateDto
            {
                TourName = "Updated Tour",
                Days = 7,
                Price = 1500.00m
            };
            var existingTour = new Tour { Id = tourId, TourName = "Old Tour" };

            _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTour);
            _mockMapper.Setup(m => m.Map(updateDto, existingTour)).Returns(existingTour);
            _mockTourRepository.Setup(r => r.UpdateAsync(existingTour, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _tourService.UpdateAsync(tourId, updateDto);

            // Assert
            _mockTourRepository.Verify(r => r.UpdateAsync(existingTour, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var tourId = 999;
            var updateDto = new TourUpdateDto { TourName = "Updated Tour", Days = 5, Price = 1000.00m };

            _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Tour?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _tourService.UpdateAsync(tourId, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyTourName_ShouldThrowBusinessException()
        {
            // Arrange
            var tourId = 1;
            var updateDto = new TourUpdateDto { TourName = "", Days = 5, Price = 1000.00m };
            var existingTour = new Tour { Id = tourId };

            _mockTourRepository.Setup(r => r.GetByIdAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingTour);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _tourService.UpdateAsync(tourId, updateDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteTour()
        {
            // Arrange
            var tourId = 1;
            _mockTourRepository.Setup(r => r.ExistsAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockTourRepository.Setup(r => r.DeleteAsync(tourId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _tourService.DeleteAsync(tourId);

            // Assert
            _mockTourRepository.Verify(r => r.DeleteAsync(tourId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var tourId = 999;
            _mockTourRepository.Setup(r => r.ExistsAsync(tourId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _tourService.DeleteAsync(tourId));
        }

        [Fact]
        public async Task SearchAsync_WithValidSearchTerm_ShouldReturnMatchingTours()
        {
            // Arrange
            var searchTerm = "Paris";
            var tours = new List<Tour> { new Tour { Id = 1, TourName = "Paris Tour" } };
            var tourDtos = new List<TourDto> { new TourDto { Id = 1, TourName = "Paris Tour" } };

            _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);
            _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
                .Returns(tourDtos);

            // Act
            var result = await _tourService.SearchAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_WithEmptySearchTerm_ShouldCallRepository()
        {
            // Arrange
            var searchTerm = "";
            var tours = new List<Tour>();
            var tourDtos = new List<TourDto>();

            _mockTourRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(tours);
            _mockMapper.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
                .Returns(tourDtos);

            // Act
            var result = await _tourService.SearchAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            _mockTourRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

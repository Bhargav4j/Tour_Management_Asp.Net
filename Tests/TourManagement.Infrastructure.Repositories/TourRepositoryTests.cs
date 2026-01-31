using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;

namespace TourManagement.Infrastructure.Repositories.Tests
{
    /// <summary>
    /// Tests for TourRepository
    /// </summary>
    public class TourRepositoryTests : IDisposable
    {
        private readonly TourManagementDbContext _context;
        private readonly Mock<ILogger<TourRepository>> _mockLogger;
        private readonly TourRepository _repository;

        public TourRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TourManagementDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TourManagementDbContext(options);
            _mockLogger = new Mock<ILogger<TourRepository>>();
            _repository = new TourRepository(_context, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullContext_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TourRepository(null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TourRepository(_context, null!));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WithNoTours_ShouldReturnEmptyList()
        {
            // Arrange
            // No tours added

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
        {
            // Arrange
            var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100 };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(tour.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tour.Id, result.Id);
            Assert.Equal("Test Tour", result.TourName);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _repository.GetByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddTour()
        {
            // Arrange
            var tour = new Tour { TourName = "New Tour", Place = "New Place", Days = 5, Price = 100 };

            // Act
            var result = await _repository.AddAsync(tour);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
            var savedTour = await _context.Tours.FindAsync(result.Id);
            Assert.NotNull(savedTour);
            Assert.Equal("New Tour", savedTour.TourName);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTour()
        {
            // Arrange
            var tour = new Tour { TourName = "Original Tour", Place = "Original Place", Days = 5, Price = 100 };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            _context.Entry(tour).State = EntityState.Detached;

            // Modify tour
            tour.TourName = "Updated Tour";
            tour.Place = "Updated Place";

            // Act
            await _repository.UpdateAsync(tour);

            // Assert
            var updatedTour = await _context.Tours.FindAsync(tour.Id);
            Assert.NotNull(updatedTour);
            Assert.Equal("Updated Tour", updatedTour.TourName);
            Assert.Equal("Updated Place", updatedTour.Place);
        }

        [Fact]
        public async Task DeleteAsync_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var tour = new Tour { TourName = "Tour to Delete", Place = "Place", Days = 5, Price = 100, IsActive = true };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(tour.Id);

            // Assert
            var deletedTour = await _context.Tours.FindAsync(tour.Id);
            Assert.NotNull(deletedTour);
            Assert.False(deletedTour.IsActive);
            Assert.NotNull(deletedTour.ModifiedDate);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldNotThrow()
        {
            // Arrange
            var invalidId = 999;

            // Act
            await _repository.DeleteAsync(invalidId);

            // Assert - No exception thrown
            Assert.True(true);
        }

        [Fact]
        public async Task ExistsAsync_WithExistingId_ShouldReturnTrue()
        {
            // Arrange
            var tour = new Tour { TourName = "Test Tour", Place = "Test Place", Days = 5, Price = 100 };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.ExistsAsync(tour.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_WithNonExistingId_ShouldReturnFalse()
        {
            // Arrange
            var invalidId = 999;

            // Act
            var result = await _repository.ExistsAsync(invalidId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task SearchAsync_WithMatchingTourName_ShouldReturnMatchingTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Paris Tour", Place = "Paris", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "London Tour", Place = "London", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("Paris");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Paris Tour", result.First().TourName);
        }

        [Fact]
        public async Task SearchAsync_WithMatchingPlace_ShouldReturnMatchingTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Tour 1", Place = "Tokyo", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "Tour 2", Place = "Osaka", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("Tokyo");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Tokyo", result.First().Place);
        }

        [Fact]
        public async Task SearchAsync_WithMatchingLocations_ShouldReturnMatchingTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Tour 1", Place = "Europe", Locations = "Eiffel Tower", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "Tour 2", Place = "Asia", Locations = "Tokyo Tower", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("Eiffel");

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Contains("Eiffel", result.First().Locations);
        }

        [Fact]
        public async Task SearchAsync_WithEmptySearchTerm_ShouldReturnAllTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_WithNullSearchTerm_ShouldReturnAllTours()
        {
            // Arrange
            var tour1 = new Tour { TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };
            var tour2 = new Tour { TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 200 };
            _context.Tours.AddRange(tour1, tour2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync(null!);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
        {
            // Arrange
            var tour = new Tour { TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 100 };
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.SearchAsync("NonExistent");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}

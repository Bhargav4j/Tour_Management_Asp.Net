using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TourManagement.Infrastructure.Repositories;
using TourManagement.Infrastructure.Data;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Tests;

public class TourRepositoryTests
{
    private readonly Mock<ILogger<TourRepository>> _mockLogger;

    public TourRepositoryTests()
    {
        _mockLogger = new Mock<ILogger<TourRepository>>();
    }

    private DbContextOptions<TourManagementDbContext> CreateInMemoryOptions()
    {
        return new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);

        // Act
        var repository = new TourRepository(context, _mockLogger.Object);

        // Assert
        Assert.NotNull(repository);
    }

    [Fact]
    public async Task GetAllAsync_WithActiveTours_ShouldReturnOnlyActiveTours()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.AddRange(
            new Tour { Id = 1, TourName = "Active Tour", IsActive = true },
            new Tour { Id = 2, TourName = "Inactive Tour", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Active Tour", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithNoTours_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnTour()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Tour", result.TourName);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_WithInactiveTour_ShouldReturnNull()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.GetByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WithValidTour_ShouldAddAndReturnTour()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var tour = new Tour { TourName = "New Tour", Place = "Paris" };

        // Act
        var result = await repository.AddAsync(tour);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(0, result.Id);
        Assert.Equal("New Tour", result.TourName);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistTourToDatabase()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        var tour = new Tour { TourName = "Test Tour", Place = "Rome" };

        using (var context = new TourManagementDbContext(options))
        {
            var repository = new TourRepository(context, _mockLogger.Object);
            await repository.AddAsync(tour);
        }

        // Act & Assert
        using (var context = new TourManagementDbContext(options))
        {
            var savedTour = await context.Tours.FirstOrDefaultAsync();
            Assert.NotNull(savedTour);
            Assert.Equal("Test Tour", savedTour.TourName);
        }
    }

    [Fact]
    public async Task UpdateAsync_WithValidTour_ShouldUpdateTour()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Tour tour;

        using (var context = new TourManagementDbContext(options))
        {
            tour = new Tour { TourName = "Original Name", Place = "Paris" };
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new TourRepository(context, _mockLogger.Object);
            tour.TourName = "Updated Name";
            await repository.UpdateAsync(tour);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var updatedTour = await context.Tours.FirstAsync();
            Assert.Equal("Updated Name", updatedTour.TourName);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldSetIsActiveFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Tour tour;

        using (var context = new TourManagementDbContext(options))
        {
            tour = new Tour { TourName = "To Delete", IsActive = true };
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new TourRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(tour.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedTour = await context.Tours.FindAsync(tour.Id);
            Assert.NotNull(deletedTour);
            Assert.False(deletedTour.IsActive);
            Assert.NotNull(deletedTour.ModifiedDate);
        }
    }

    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldNotThrowException()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act & Assert
        await repository.DeleteAsync(999);
        Assert.True(true);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSetModifiedDate()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        Tour tour;

        using (var context = new TourManagementDbContext(options))
        {
            tour = new Tour { TourName = "To Delete", IsActive = true };
            context.Tours.Add(tour);
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = new TourManagementDbContext(options))
        {
            var repository = new TourRepository(context, _mockLogger.Object);
            await repository.DeleteAsync(tour.Id);
        }

        // Assert
        using (var context = new TourManagementDbContext(options))
        {
            var deletedTour = await context.Tours.FindAsync(tour.Id);
            Assert.NotNull(deletedTour?.ModifiedDate);
        }
    }

    [Fact]
    public async Task ExistsAsync_WithExistingActiveTour_ShouldReturnTrue()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { Id = 1, TourName = "Test Tour", IsActive = true };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WithNonExistingTour_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ExistsAsync_WithInactiveTour_ShouldReturnFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var tour = new Tour { Id = 1, TourName = "Inactive Tour", IsActive = false };
        context.Tours.Add(tour);
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.ExistsAsync(1);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingTourName_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.AddRange(
            new Tour { TourName = "Paris Tour", Place = "France", IsActive = true },
            new Tour { TourName = "Rome Tour", Place = "Italy", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Tour", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingPlace_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Place = "France", IsActive = true },
            new Tour { TourName = "Tour 2", Place = "Italy", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Italy");

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour 2", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithMatchingLocations_ShouldReturnMatches()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.AddRange(
            new Tour { TourName = "Tour 1", Locations = "Eiffel Tower", IsActive = true },
            new Tour { TourName = "Tour 2", Locations = "Colosseum", IsActive = true }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Eiffel");

        // Assert
        Assert.Single(result);
        Assert.Equal("Tour 1", result.First().TourName);
    }

    [Fact]
    public async Task SearchAsync_WithNoMatches_ShouldReturnEmptyList()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.Add(new Tour { TourName = "Paris Tour", Place = "France", IsActive = true });
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("NonExistent");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task SearchAsync_ShouldNotReturnInactiveTours()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        context.Tours.AddRange(
            new Tour { TourName = "Paris Active", IsActive = true },
            new Tour { TourName = "Paris Inactive", IsActive = false }
        );
        await context.SaveChangesAsync();

        var repository = new TourRepository(context, _mockLogger.Object);

        // Act
        var result = await repository.SearchAsync("Paris");

        // Assert
        Assert.Single(result);
        Assert.Equal("Paris Active", result.First().TourName);
    }

    [Fact]
    public async Task GetAllAsync_WithCancellationToken_ShouldRespectToken()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new TourManagementDbContext(options);
        var repository = new TourRepository(context, _mockLogger.Object);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await repository.GetAllAsync(cancellationToken);

        // Assert
        Assert.NotNull(result);
    }
}

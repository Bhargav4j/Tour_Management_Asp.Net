using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.Entities;
using TourManagement.Infrastructure.Data;
using TourManagement.Infrastructure.Repositories;
using Xunit;

namespace TourManagement.IntegrationTests.Repositories;

public class TourRepositoryTests : IDisposable
{
    private readonly TourManagementDbContext _context;
    private readonly TourRepository _repository;

    public TourRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<TourManagementDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TourManagementDbContext(options);
        var mockLogger = new Mock<ILogger<TourRepository>>();
        _repository = new TourRepository(_context, mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_AddsTourToDatabase()
    {
        var tour = new Tour
        {
            Name = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            Locations = "Location 1, Location 2",
            TourInfo = "Test information",
            IsActive = true,
            CreatedBy = "test",
            CreatedDate = DateTime.UtcNow
        };

        var result = await _repository.AddAsync(tour);

        result.Id.Should().BeGreaterThan(0);
        var savedTour = await _context.Tours.FindAsync(result.Id);
        savedTour.Should().NotBeNull();
        savedTour!.Name.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyActiveTours()
    {
        _context.Tours.AddRange(
            new Tour { Name = "Active Tour", Place = "Place", Days = 3, Price = 500, Locations = "A", TourInfo = "Info", IsActive = true, CreatedBy = "test", CreatedDate = DateTime.UtcNow },
            new Tour { Name = "Inactive Tour", Place = "Place", Days = 3, Price = 500, Locations = "B", TourInfo = "Info", IsActive = false, CreatedBy = "test", CreatedDate = DateTime.UtcNow }
        );
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Active Tour");
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

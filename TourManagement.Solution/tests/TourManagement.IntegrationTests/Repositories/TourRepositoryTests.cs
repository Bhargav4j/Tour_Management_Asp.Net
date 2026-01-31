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
    public async Task AddAsync_ShouldAddTourToDatabase()
    {
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            Locations = "Location1, Location2",
            TourInfo = "Test Info",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "Test"
        };

        var result = await _repository.AddAsync(tour);

        result.Id.Should().BeGreaterThan(0);
        result.TourName.Should().Be("Test Tour");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveTours()
    {
        var tour1 = new Tour
        {
            TourName = "Tour 1",
            Place = "Place 1",
            Days = 3,
            Price = 500,
            Locations = "Loc1",
            TourInfo = "Info1",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "Test"
        };

        var tour2 = new Tour
        {
            TourName = "Tour 2",
            Place = "Place 2",
            Days = 7,
            Price = 1500,
            Locations = "Loc2",
            TourInfo = "Info2",
            CreatedDate = DateTime.UtcNow,
            IsActive = true,
            CreatedBy = "Test"
        };

        await _repository.AddAsync(tour1);
        await _repository.AddAsync(tour2);

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

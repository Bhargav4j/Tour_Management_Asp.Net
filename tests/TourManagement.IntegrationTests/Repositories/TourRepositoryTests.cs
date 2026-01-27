using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
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
        var logger = NullLogger<TourRepository>.Instance;
        _repository = new TourRepository(_context, logger);
    }

    [Fact]
    public async Task AddAsync_ShouldAddTour()
    {
        var tour = new Tour
        {
            TourName = "Test Tour",
            Place = "Test Place",
            Days = 5,
            Price = 1000,
            Locations = "Location 1",
            TourInfo = "Test Info",
            IsActive = true
        };

        var result = await _repository.AddAsync(tour);

        result.TourId.Should().BeGreaterThan(0);
        result.TourName.Should().Be("Test Tour");

        var savedTour = await _context.Tours.FindAsync(result.TourId);
        savedTour.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllActiveTours()
    {
        var tours = new List<Tour>
        {
            new Tour { TourName = "Tour 1", Place = "Place 1", Days = 5, Price = 1000, Locations = "Loc1", TourInfo = "Info1", IsActive = true },
            new Tour { TourName = "Tour 2", Place = "Place 2", Days = 7, Price = 1500, Locations = "Loc2", TourInfo = "Info2", IsActive = true },
            new Tour { TourName = "Tour 3", Place = "Place 3", Days = 3, Price = 500, Locations = "Loc3", TourInfo = "Info3", IsActive = false }
        };

        await _context.Tours.AddRangeAsync(tours);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().OnlyContain(t => t.IsActive);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

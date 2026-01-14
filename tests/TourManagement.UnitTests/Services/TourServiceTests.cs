using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Domain.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

public class TourServiceTests
{
    private readonly Mock<ITourRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TourService>> _loggerMock;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _repositoryMock = new Mock<ITourRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TourService>>();
        _service = new TourService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllTours()
    {
        var tours = new List<Tour>
        {
            new Tour { Id = 1, TourName = "Tour1" },
            new Tour { Id = 2, TourName = "Tour2" }
        };
        var tourDtos = new List<TourDto>
        {
            new TourDto { Id = 1, TourName = "Tour1" },
            new TourDto { Id = 2, TourName = "Tour2" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(tours);
        _mapperMock.Setup(m => m.Map<IEnumerable<TourDto>>(tours))
            .Returns(tourDtos);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
        result.Should().BeEquivalentTo(tourDtos);
    }
}

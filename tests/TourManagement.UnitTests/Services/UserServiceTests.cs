using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using Xunit;

namespace TourManagement.UnitTests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _service = new UserService(_mockRepository.Object, _mockMapper.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = 1, Email = "test1@test.com", FirstName = "Test1", IsActive = true },
            new User { Id = 2, Email = "test2@test.com", FirstName = "Test2", IsActive = true }
        };
        var userDtos = new List<UserDto>
        {
            new UserDto { Id = 1, Email = "test1@test.com", FirstName = "Test1" },
            new UserDto { Id = 2, Email = "test2@test.com", FirstName = "Test2" }
        };

        _mockRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(users);
        _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
            .Returns(userDtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var userDto = new UserDto { Id = 1, Email = "test@test.com", FirstName = "Test" };

        _mockRepository.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Email.Should().Be("test@test.com");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_CreatesUser()
    {
        // Arrange
        var createDto = new UserCreateDto
        {
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User",
            Password = "Password123"
        };
        var user = new User { Id = 1, Email = "test@test.com", FirstName = "Test", IsActive = true };
        var userDto = new UserDto { Id = 1, Email = "test@test.com", FirstName = "Test" };

        _mockRepository.Setup(r => r.EmailExistsAsync(createDto.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _mockMapper.Setup(m => m.Map<User>(createDto))
            .Returns(user);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _service.CreateAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(createDto.Email);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

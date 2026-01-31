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
    /// Tests for UserService
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<UserService>> _mockLogger;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _userService = new UserService(_mockUserRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullUserRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UserService(null!, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UserService(_mockUserRepository.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new UserService(_mockUserRepository.Object, _mockMapper.Object, null!));
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User> { new User { Id = 1 }, new User { Id = 2 } };
            var userDtos = new List<UserDto> { new UserDto { Id = 1 }, new UserDto { Id = 2 } };

            _mockUserRepository.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _userService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<UserDto>)result).Count);
            _mockUserRepository.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Email = "test@example.com" };
            var userDto = new UserDto { Id = userId, Email = "test@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("test@example.com", result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.GetByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_WithValidDto_ShouldReturnCreatedUser()
        {
            // Arrange
            var createDto = new UserCreateDto
            {
                Email = "newuser@example.com",
                Password = "Password123"
            };
            var user = new User { Id = 1, Email = "newuser@example.com" };
            var userDto = new UserDto { Id = 1, Email = "newuser@example.com" };

            _mockUserRepository.Setup(r => r.EmailExistsAsync(createDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map<User>(createDto)).Returns(user);
            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

            // Act
            var result = await _userService.RegisterAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("newuser@example.com", result.Email);
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyEmail_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new UserCreateDto
            {
                Email = "",
                Password = "Password123"
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.RegisterAsync(createDto));
        }

        [Fact]
        public async Task RegisterAsync_WithEmptyPassword_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new UserCreateDto
            {
                Email = "test@example.com",
                Password = ""
            };

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.RegisterAsync(createDto));
        }

        [Fact]
        public async Task RegisterAsync_WithExistingEmail_ShouldThrowBusinessException()
        {
            // Arrange
            var createDto = new UserCreateDto
            {
                Email = "existing@example.com",
                Password = "Password123"
            };

            _mockUserRepository.Setup(r => r.EmailExistsAsync(createDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.RegisterAsync(createDto));
        }

        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var email = "user@example.com";
            var password = "Password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword };
            var userDto = new UserDto { Id = 1, Email = email };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _mockMapper.Setup(m => m.Map<UserDto>(user))
                .Returns(userDto);

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexistent@example.com";
            var password = "Password123";

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
        {
            // Arrange
            var email = "user@example.com";
            var password = "WrongPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
            var user = new User { Id = 1, Email = email, PasswordHash = hashedPassword };

            _mockUserRepository.Setup(r => r.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithEmptyEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "";
            var password = "Password123";

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_WithEmptyPassword_ShouldReturnNull()
        {
            // Arrange
            var email = "user@example.com";
            var password = "";

            // Act
            var result = await _userService.LoginAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_WithValidDto_ShouldUpdateUser()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UserUpdateDto
            {
                Email = "updated@example.com",
                FullName = "Updated Name"
            };
            var existingUser = new User { Id = userId, Email = "old@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);
            _mockMapper.Setup(m => m.Map(updateDto, existingUser)).Returns(existingUser);
            _mockUserRepository.Setup(r => r.UpdateAsync(existingUser, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.UpdateAsync(userId, updateDto);

            // Assert
            _mockUserRepository.Verify(r => r.UpdateAsync(existingUser, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var userId = 999;
            var updateDto = new UserUpdateDto { Email = "test@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.UpdateAsync(userId, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyEmail_ShouldThrowBusinessException()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UserUpdateDto { Email = "" };
            var existingUser = new User { Id = userId, Email = "old@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.UpdateAsync(userId, updateDto));
        }

        [Fact]
        public async Task UpdateAsync_WithExistingEmail_ShouldThrowBusinessException()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UserUpdateDto { Email = "existing@example.com" };
            var existingUser = new User { Id = userId, Email = "old@example.com" };

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.EmailExistsAsync(updateDto.Email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BusinessException>(() => _userService.UpdateAsync(userId, updateDto));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldDeleteUser()
        {
            // Arrange
            var userId = 1;
            _mockUserRepository.Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
            _mockUserRepository.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.DeleteAsync(userId);

            // Assert
            _mockUserRepository.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithInvalidId_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var userId = 999;
            _mockUserRepository.Setup(r => r.ExistsAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _userService.DeleteAsync(userId));
        }

        [Fact]
        public async Task SearchAsync_WithValidSearchTerm_ShouldReturnMatchingUsers()
        {
            // Arrange
            var searchTerm = "test";
            var users = new List<User> { new User { Id = 1, Email = "test@example.com" } };
            var userDtos = new List<UserDto> { new UserDto { Id = 1, Email = "test@example.com" } };

            _mockUserRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _userService.SearchAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_WithEmptySearchTerm_ShouldCallRepository()
        {
            // Arrange
            var searchTerm = "";
            var users = new List<User>();
            var userDtos = new List<UserDto>();

            _mockUserRepository.Setup(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);
            _mockMapper.Setup(m => m.Map<IEnumerable<UserDto>>(users))
                .Returns(userDtos);

            // Act
            var result = await _userService.SearchAsync(searchTerm);

            // Assert
            Assert.NotNull(result);
            _mockUserRepository.Verify(r => r.SearchAsync(searchTerm, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}

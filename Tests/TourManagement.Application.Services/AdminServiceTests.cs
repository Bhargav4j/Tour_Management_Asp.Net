using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services.Tests
{
    /// <summary>
    /// Tests for AdminService
    /// </summary>
    public class AdminServiceTests
    {
        private readonly Mock<IAdminRepository> _mockAdminRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ILogger<AdminService>> _mockLogger;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _mockAdminRepository = new Mock<IAdminRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_mockAdminRepository.Object, _mockMapper.Object, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_WithNullAdminRepository_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AdminService(null!, _mockMapper.Object, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullMapper_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AdminService(_mockAdminRepository.Object, null!, _mockLogger.Object));
        }

        [Fact]
        public void Constructor_WithNullLogger_ShouldThrowArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new AdminService(_mockAdminRepository.Object, _mockMapper.Object, null!));
        }

        [Fact]
        public async Task ValidateLoginAsync_WithValidCredentials_ShouldReturnTrue()
        {
            // Arrange
            var username = "admin";
            var password = "password123";

            _mockAdminRepository.Setup(r => r.ValidateCredentialsAsync(username, password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.True(result);
            _mockAdminRepository.Verify(r => r.ValidateCredentialsAsync(username, password, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithInvalidCredentials_ShouldReturnFalse()
        {
            // Arrange
            var username = "admin";
            var password = "wrongpassword";

            _mockAdminRepository.Setup(r => r.ValidateCredentialsAsync(username, password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithEmptyUsername_ShouldReturnFalse()
        {
            // Arrange
            var username = "";
            var password = "password123";

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.False(result);
            _mockAdminRepository.Verify(r => r.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var username = "admin";
            var password = "";

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.False(result);
            _mockAdminRepository.Verify(r => r.ValidateCredentialsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithNullUsername_ShouldReturnFalse()
        {
            // Arrange
            string? username = null;
            var password = "password123";

            // Act
            var result = await _adminService.ValidateLoginAsync(username!, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithNullPassword_ShouldReturnFalse()
        {
            // Arrange
            var username = "admin";
            string? password = null;

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithWhitespaceUsername_ShouldReturnFalse()
        {
            // Arrange
            var username = "   ";
            var password = "password123";

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateLoginAsync_WithWhitespacePassword_ShouldReturnFalse()
        {
            // Arrange
            var username = "admin";
            var password = "   ";

            // Act
            var result = await _adminService.ValidateLoginAsync(username, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithValidUsername_ShouldReturnAdmin()
        {
            // Arrange
            var username = "admin";
            var admin = new Admin { Id = 1, Username = username, Email = "admin@test.com" };
            var adminDto = new AdminDto { Id = 1, Username = username, Email = "admin@test.com" };

            _mockAdminRepository.Setup(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(admin);
            _mockMapper.Setup(m => m.Map<AdminDto>(admin))
                .Returns(adminDto);

            // Act
            var result = await _adminService.GetByUsernameAsync(username);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.Username);
            Assert.Equal("admin@test.com", result.Email);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithInvalidUsername_ShouldReturnNull()
        {
            // Arrange
            var username = "nonexistent";

            _mockAdminRepository.Setup(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Admin?)null);

            // Act
            var result = await _adminService.GetByUsernameAsync(username);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByUsernameAsync_WithEmptyUsername_ShouldCallRepository()
        {
            // Arrange
            var username = "";

            _mockAdminRepository.Setup(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Admin?)null);

            // Act
            var result = await _adminService.GetByUsernameAsync(username);

            // Assert
            Assert.Null(result);
            _mockAdminRepository.Verify(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByUsernameAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var username = "admin";
            _mockAdminRepository.Setup(r => r.GetByUsernameAsync(username, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _adminService.GetByUsernameAsync(username));
        }

        [Fact]
        public async Task ValidateLoginAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var username = "admin";
            var password = "password123";
            _mockAdminRepository.Setup(r => r.ValidateCredentialsAsync(username, password, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _adminService.ValidateLoginAsync(username, password));
        }
    }
}

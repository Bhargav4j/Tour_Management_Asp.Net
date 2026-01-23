using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Web.Pages.Account;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Web.Tests.Pages.Account;

public class LoginModelTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;
    private readonly LoginModel _loginModel;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<ISession> _mockSession;

    public LoginModelTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<LoginModel>>();
        _mockHttpContext = new Mock<HttpContext>();
        _mockSession = new Mock<ISession>();

        _mockHttpContext.Setup(x => x.Session).Returns(_mockSession.Object);

        _loginModel = new LoginModel(_mockUserService.Object, _mockLogger.Object)
        {
            PageContext = new PageContext { HttpContext = _mockHttpContext.Object }
        };
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateInstance()
    {
        // Arrange & Act & Assert
        Assert.NotNull(_loginModel);
    }

    [Fact]
    public void Email_Property_ShouldBeInitializedToEmptyString()
    {
        // Assert
        Assert.Equal(string.Empty, _loginModel.Email);
    }

    [Fact]
    public void Password_Property_ShouldBeInitializedToEmptyString()
    {
        // Assert
        Assert.Equal(string.Empty, _loginModel.Password);
    }

    [Fact]
    public void OnGet_ShouldLogInformation()
    {
        // Act
        _loginModel.OnGet();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Login page accessed")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidModelState_ShouldReturnPage()
    {
        // Arrange
        _loginModel.ModelState.AddModelError("Email", "Email is required");

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        _mockUserService.Verify(x => x.AuthenticateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task OnPostAsync_WithValidCredentials_ShouldRedirectToTourIndex()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe"
        };
        _loginModel.Email = "test@test.com";
        _loginModel.Password = "password123";

        byte[] sessionValue = Array.Empty<byte>();
        _mockSession.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Callback<string, byte[]>((key, value) => sessionValue = value);

        _mockUserService.Setup(x => x.AuthenticateAsync("test@test.com", "password123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        var redirectResult = Assert.IsType<RedirectToPageResult>(result);
        Assert.Equal("/Tours/Index", redirectResult.PageName);
        _mockSession.Verify(x => x.Set("UserId", It.IsAny<byte[]>()), Times.Once);
        _mockSession.Verify(x => x.Set("UserEmail", It.IsAny<byte[]>()), Times.Once);
        _mockSession.Verify(x => x.Set("UserName", It.IsAny<byte[]>()), Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithInvalidCredentials_ShouldReturnPageWithError()
    {
        // Arrange
        _loginModel.Email = "test@test.com";
        _loginModel.Password = "wrongpassword";

        _mockUserService.Setup(x => x.AuthenticateAsync("test@test.com", "wrongpassword", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid email or password", _loginModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WithException_ShouldReturnPageWithError()
    {
        // Arrange
        _loginModel.Email = "test@test.com";
        _loginModel.Password = "password123";

        _mockUserService.Setup(x => x.AuthenticateAsync("test@test.com", "password123", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _loginModel.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.NotNull(_loginModel.ErrorMessage);
        Assert.Contains("error occurred", _loginModel.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_WithNullUser_ShouldLogWarning()
    {
        // Arrange
        _loginModel.Email = "test@test.com";
        _loginModel.Password = "wrongpassword";

        _mockUserService.Setup(x => x.AuthenticateAsync("test@test.com", "wrongpassword", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        await _loginModel.OnPostAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Failed login attempt")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task OnPostAsync_WithValidCredentials_ShouldLogSuccess()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe"
        };
        _loginModel.Email = "test@test.com";
        _loginModel.Password = "password123";

        _mockUserService.Setup(x => x.AuthenticateAsync("test@test.com", "password123", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        await _loginModel.OnPostAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("logged in successfully")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public void Email_Property_CanBeSet()
    {
        // Act
        _loginModel.Email = "newemail@test.com";

        // Assert
        Assert.Equal("newemail@test.com", _loginModel.Email);
    }

    [Fact]
    public void Password_Property_CanBeSet()
    {
        // Act
        _loginModel.Password = "newpassword123";

        // Assert
        Assert.Equal("newpassword123", _loginModel.Password);
    }

    [Fact]
    public void ErrorMessage_Property_CanBeSet()
    {
        // Act
        _loginModel.ErrorMessage = "Test error";

        // Assert
        Assert.Equal("Test error", _loginModel.ErrorMessage);
    }

    [Fact]
    public void SuccessMessage_Property_CanBeSet()
    {
        // Act
        _loginModel.SuccessMessage = "Test success";

        // Assert
        Assert.Equal("Test success", _loginModel.SuccessMessage);
    }
}

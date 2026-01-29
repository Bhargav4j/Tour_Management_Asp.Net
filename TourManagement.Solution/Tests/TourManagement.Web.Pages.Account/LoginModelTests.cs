using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using TourManagement.Web.Pages.Account;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Web.Pages.Account;

public class LoginModelTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<LoginModel>> _mockLogger;

    public LoginModelTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<LoginModel>>();
    }

    [Fact]
    public void Constructor_WithNullUserService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new LoginModel(_mockUserService.Object, null!));
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        var model = new LoginModel(_mockUserService.Object, _mockLogger.Object);

        model.OnGet();

        Assert.NotNull(model.Input);
    }

    [Fact]
    public void InputModel_InitializesWithDefaultValues()
    {
        var input = new LoginModel.InputModel();

        Assert.Equal(string.Empty, input.Email);
        Assert.Equal(string.Empty, input.Password);
    }

    [Fact]
    public void InputModel_SetProperties_ShouldStoreValues()
    {
        var input = new LoginModel.InputModel
        {
            Email = "test@example.com",
            Password = "password123"
        };

        Assert.Equal("test@example.com", input.Email);
        Assert.Equal("password123", input.Password);
    }
}

using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using TourManagement.Web.Pages.Account;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Tests.Web.Pages.Account;

public class RegisterModelTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ILogger<RegisterModel>> _mockLogger;

    public RegisterModelTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockLogger = new Mock<ILogger<RegisterModel>>();
    }

    [Fact]
    public void Constructor_WithNullUserService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RegisterModel(null!, _mockLogger.Object));
    }

    [Fact]
    public void Constructor_WithNullLogger_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new RegisterModel(_mockUserService.Object, null!));
    }

    [Fact]
    public void OnGet_ExecutesSuccessfully()
    {
        var model = new RegisterModel(_mockUserService.Object, _mockLogger.Object);

        model.OnGet();

        Assert.NotNull(model.Input);
    }

    [Fact]
    public void InputModel_InitializesWithDefaultValues()
    {
        var input = new RegisterModel.InputModel();

        Assert.Equal(string.Empty, input.Email);
        Assert.Equal(string.Empty, input.Password);
        Assert.Equal(string.Empty, input.FirstName);
        Assert.Equal(string.Empty, input.LastName);
        Assert.Equal(string.Empty, input.Gender);
        Assert.Equal(string.Empty, input.Street);
        Assert.Equal(string.Empty, input.City);
        Assert.Equal(string.Empty, input.State);
    }

    [Fact]
    public void InputModel_SetProperties_ShouldStoreValues()
    {
        var input = new RegisterModel.InputModel
        {
            Email = "newuser@example.com",
            Password = "SecurePass123",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            DateOfBirth = new DateTime(1990, 5, 15),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        Assert.Equal("newuser@example.com", input.Email);
        Assert.Equal("SecurePass123", input.Password);
        Assert.Equal("John", input.FirstName);
        Assert.Equal("Doe", input.LastName);
        Assert.Equal("Male", input.Gender);
        Assert.Equal(new DateTime(1990, 5, 15), input.DateOfBirth);
        Assert.Equal("123 Main St", input.Street);
        Assert.Equal("New York", input.City);
        Assert.Equal("NY", input.State);
    }
}

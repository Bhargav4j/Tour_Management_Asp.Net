using Xunit;
using TourManagement.Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace TourManagement.Web.ViewModels.Tests;

public class LoginViewModelTests
{
    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var viewModel = new LoginViewModel();

        // Act
        viewModel.Email = "test@test.com";
        viewModel.Password = "password123";

        // Assert
        Assert.Equal("test@test.com", viewModel.Email);
        Assert.Equal("password123", viewModel.Password);
    }

    [Fact]
    public void Validation_FailsWhenEmailIsEmpty()
    {
        // Arrange
        var viewModel = new LoginViewModel { Email = "", Password = "password123" };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void Validation_FailsWhenPasswordIsEmpty()
    {
        // Arrange
        var viewModel = new LoginViewModel { Email = "test@test.com", Password = "" };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Password"));
    }

    [Fact]
    public void Validation_PassesWhenAllFieldsValid()
    {
        // Arrange
        var viewModel = new LoginViewModel { Email = "test@test.com", Password = "password123" };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}

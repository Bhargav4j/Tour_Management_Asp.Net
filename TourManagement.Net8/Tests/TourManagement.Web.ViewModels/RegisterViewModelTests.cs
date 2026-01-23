using Xunit;
using TourManagement.Web.ViewModels;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace TourManagement.Web.ViewModels.Tests;

public class RegisterViewModelTests
{
    [Fact]
    public void Properties_CanBeSet()
    {
        // Arrange
        var viewModel = new RegisterViewModel();
        var dob = new DateTime(1990, 1, 1);

        // Act
        viewModel.Email = "test@test.com";
        viewModel.FirstName = "John";
        viewModel.LastName = "Doe";
        viewModel.Gender = "Male";
        viewModel.Password = "password123";
        viewModel.DateOfBirth = dob;
        viewModel.Street = "123 Main St";
        viewModel.City = "New York";
        viewModel.State = "NY";

        // Assert
        Assert.Equal("test@test.com", viewModel.Email);
        Assert.Equal("John", viewModel.FirstName);
        Assert.Equal("Doe", viewModel.LastName);
        Assert.Equal("Male", viewModel.Gender);
        Assert.Equal("password123", viewModel.Password);
        Assert.Equal(dob, viewModel.DateOfBirth);
        Assert.Equal("123 Main St", viewModel.Street);
        Assert.Equal("New York", viewModel.City);
        Assert.Equal("NY", viewModel.State);
    }

    [Fact]
    public void Validation_FailsWhenEmailIsEmpty()
    {
        // Arrange
        var viewModel = new RegisterViewModel
        {
            Email = "",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            DateOfBirth = DateTime.Now,
            Street = "Street",
            City = "City",
            State = "State"
        };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.False(isValid);
        Assert.Contains(results, r => r.MemberNames.Contains("Email"));
    }

    [Fact]
    public void Validation_FailsWhenPasswordTooShort()
    {
        // Arrange
        var viewModel = new RegisterViewModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "abc",
            DateOfBirth = DateTime.Now,
            Street = "Street",
            City = "City",
            State = "State"
        };
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
        var viewModel = new RegisterViewModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            DateOfBirth = DateTime.Now,
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };
        var context = new ValidationContext(viewModel);
        var results = new List<ValidationResult>();

        // Act
        var isValid = Validator.TryValidateObject(viewModel, context, results, true);

        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}

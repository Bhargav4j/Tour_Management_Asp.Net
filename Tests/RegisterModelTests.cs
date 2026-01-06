using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Users;

namespace TourManagement.Web.Pages.Users.Tests;

public class RegisterModelTests
{
    private class MockUserService : IUserService
    {
        public List<User> Users { get; set; } = new List<User>();
        public bool ThrowException { get; set; }
        public bool ThrowInvalidOperationException { get; set; }

        public Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.AsEnumerable());
        }

        public Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Id == id));
        }

        public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default)
        {
            if (ThrowException) throw new Exception("Test exception");
            if (ThrowInvalidOperationException) throw new InvalidOperationException("Email already exists");
            user.Id = Users.Count + 1;
            Users.Add(user);
            return Task.FromResult(user);
        }

        public Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Email == email));
        }

        public Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.Where(u => u.Email.Contains(searchTerm)).AsEnumerable());
        }
    }

    private class MockLogger : ILogger<RegisterModel>
    {
        public int LogErrorCallCount { get; private set; }

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Error)
            {
                LogErrorCallCount++;
            }
        }
    }

    [Fact]
    public void Constructor_InitializesModel()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();

        // Act
        var model = new RegisterModel(userService, logger);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);

        // Act & Assert (should not throw)
        model.OnGet();
    }

    [Fact]
    public void Input_InitializesToNewInputModel()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();

        // Act
        var model = new RegisterModel(userService, logger);

        // Assert
        Assert.NotNull(model.Input);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_CreatesUser_WithValidInput()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Single(userService.Users);
        Assert.Equal("test@test.com", userService.Users.First().Email);
    }

    [Fact]
    public async Task OnPostAsync_CreatesAndMapsUserData_OnSuccess()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Single(userService.Users);
        Assert.Equal("test@test.com", userService.Users.First().Email);
    }

    [Fact]
    public async Task OnPostAsync_SetsErrorMessage_OnInvalidOperationException()
    {
        // Arrange
        var userService = new MockUserService { ThrowInvalidOperationException = true };
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Equal("Email already exists", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_SetsErrorMessage_OnGeneralException()
    {
        // Arrange
        var userService = new MockUserService { ThrowException = true };
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Equal("An error occurred during registration. Please try again.", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_LogsError_OnGeneralException()
    {
        // Arrange
        var userService = new MockUserService { ThrowException = true };
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        await model.OnPostAsync();

        // Assert
        Assert.Equal(1, logger.LogErrorCallCount);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_OnException()
    {
        // Arrange
        var userService = new MockUserService { ThrowException = true };
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        model.Input = new RegisterModel.InputModel
        {
            Email = "test@test.com",
            FirstName = "John",
            LastName = "Doe",
            Gender = "Male",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = new DateTime(1990, 1, 1),
            Street = "123 Main St",
            City = "New York",
            State = "NY"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public void InputModel_HasRequiredProperties()
    {
        // Arrange & Act
        var inputModel = new RegisterModel.InputModel();

        // Assert
        Assert.NotNull(inputModel.Email);
        Assert.NotNull(inputModel.FirstName);
        Assert.NotNull(inputModel.LastName);
        Assert.NotNull(inputModel.Gender);
        Assert.NotNull(inputModel.Password);
        Assert.NotNull(inputModel.ConfirmPassword);
        Assert.NotNull(inputModel.Street);
        Assert.NotNull(inputModel.City);
        Assert.NotNull(inputModel.State);
    }

    [Fact]
    public void InputModel_CanSetAllProperties()
    {
        // Arrange
        var inputModel = new RegisterModel.InputModel();
        var dob = new DateTime(1985, 5, 15);

        // Act
        inputModel.Email = "test@test.com";
        inputModel.FirstName = "Jane";
        inputModel.LastName = "Smith";
        inputModel.Gender = "Female";
        inputModel.Password = "pass123";
        inputModel.ConfirmPassword = "pass123";
        inputModel.DateOfBirth = dob;
        inputModel.Street = "456 Elm St";
        inputModel.City = "Boston";
        inputModel.State = "MA";

        // Assert
        Assert.Equal("test@test.com", inputModel.Email);
        Assert.Equal("Jane", inputModel.FirstName);
        Assert.Equal("Smith", inputModel.LastName);
        Assert.Equal("Female", inputModel.Gender);
        Assert.Equal("pass123", inputModel.Password);
        Assert.Equal("pass123", inputModel.ConfirmPassword);
        Assert.Equal(dob, inputModel.DateOfBirth);
        Assert.Equal("456 Elm St", inputModel.Street);
        Assert.Equal("Boston", inputModel.City);
        Assert.Equal("MA", inputModel.State);
    }

    [Fact]
    public async Task OnPostAsync_MapsUserProperties_Correctly()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new RegisterModel(userService, logger);
        var dob = new DateTime(1995, 3, 20);
        model.Input = new RegisterModel.InputModel
        {
            Email = "mapped@test.com",
            FirstName = "Mapped",
            LastName = "User",
            Gender = "Other",
            Password = "password123",
            ConfirmPassword = "password123",
            DateOfBirth = dob,
            Street = "789 Oak St",
            City = "Chicago",
            State = "IL"
        };

        // Act
        await model.OnPostAsync();

        // Assert
        var createdUser = userService.Users.First();
        Assert.Equal("mapped@test.com", createdUser.Email);
        Assert.Equal("Mapped", createdUser.FirstName);
        Assert.Equal("User", createdUser.LastName);
        Assert.Equal("Other", createdUser.Gender);
        Assert.Equal(dob, createdUser.DateOfBirth);
        Assert.Equal("789 Oak St", createdUser.Street);
        Assert.Equal("Chicago", createdUser.City);
        Assert.Equal("IL", createdUser.State);
    }

    [Fact]
    public void ErrorMessage_InitializesToNull()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();

        // Act
        var model = new RegisterModel(userService, logger);

        // Assert
        Assert.Null(model.ErrorMessage);
    }
}

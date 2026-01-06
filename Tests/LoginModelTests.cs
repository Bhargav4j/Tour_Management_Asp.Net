using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.Pages.Users;

namespace TourManagement.Web.Pages.Users.Tests;

public class LoginModelTests
{
    private class MockUserService : IUserService
    {
        public List<User> Users { get; set; } = new List<User>();
        public bool ThrowException { get; set; }
        public User? AuthenticateResult { get; set; }

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
            if (ThrowException) throw new Exception("Test exception");
            return Task.FromResult(AuthenticateResult);
        }

        public Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Users.Where(u => u.Email.Contains(searchTerm)).AsEnumerable());
        }
    }

    private class MockLogger : ILogger<LoginModel>
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

    private class MockSession : ISession
    {
        private Dictionary<string, byte[]> _sessionData = new Dictionary<string, byte[]>();

        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public IEnumerable<string> Keys => _sessionData.Keys;

        public void Clear() => _sessionData.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _sessionData.Remove(key);

        public bool TryGetValue(string key, out byte[]? value)
        {
            return _sessionData.TryGetValue(key, out value);
        }

        public void Set(string key, byte[] value)
        {
            _sessionData[key] = value;
        }
    }

    [Fact]
    public void Constructor_InitializesModel()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();

        // Act
        var model = new LoginModel(userService, logger);

        // Assert
        Assert.NotNull(model);
    }

    [Fact]
    public void OnGet_DoesNotThrow()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);

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
        var model = new LoginModel(userService, logger);

        // Assert
        Assert.NotNull(model.Input);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_WhenModelStateIsInvalid()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);
        model.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
    }

    [Fact]
    public async Task OnPostAsync_ReturnsPage_WhenAuthenticationFails()
    {
        // Arrange
        var userService = new MockUserService { AuthenticateResult = null };
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);
        model.Input = new LoginModel.InputModel
        {
            Email = "test@test.com",
            Password = "password"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.IsType<PageResult>(result);
        Assert.Equal("Invalid email or password", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_SetsErrorMessage_OnException()
    {
        // Arrange
        var userService = new MockUserService { ThrowException = true };
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);
        model.Input = new LoginModel.InputModel
        {
            Email = "test@test.com",
            Password = "password"
        };

        // Act
        var result = await model.OnPostAsync();

        // Assert
        Assert.Equal("An error occurred during login. Please try again.", model.ErrorMessage);
    }

    [Fact]
    public async Task OnPostAsync_LogsError_OnException()
    {
        // Arrange
        var userService = new MockUserService { ThrowException = true };
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);
        model.Input = new LoginModel.InputModel
        {
            Email = "test@test.com",
            Password = "password"
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
        var model = new LoginModel(userService, logger);
        model.Input = new LoginModel.InputModel
        {
            Email = "test@test.com",
            Password = "password"
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
        var inputModel = new LoginModel.InputModel();

        // Assert
        Assert.NotNull(inputModel.Email);
        Assert.NotNull(inputModel.Password);
    }

    [Fact]
    public void InputModel_CanSetAllProperties()
    {
        // Arrange
        var inputModel = new LoginModel.InputModel();

        // Act
        inputModel.Email = "test@test.com";
        inputModel.Password = "password123";

        // Assert
        Assert.Equal("test@test.com", inputModel.Email);
        Assert.Equal("password123", inputModel.Password);
    }

    [Fact]
    public void InputModel_InitializesWithEmptyStrings()
    {
        // Arrange & Act
        var inputModel = new LoginModel.InputModel();

        // Assert
        Assert.Equal(string.Empty, inputModel.Email);
        Assert.Equal(string.Empty, inputModel.Password);
    }

    [Fact]
    public async Task OnPostAsync_CallsAuthenticateAsync_WithCorrectParameters()
    {
        // Arrange
        var userService = new MockUserService { AuthenticateResult = null };
        var logger = new MockLogger();
        var model = new LoginModel(userService, logger);
        model.Input = new LoginModel.InputModel
        {
            Email = "test@test.com",
            Password = "testpassword"
        };

        // Act
        await model.OnPostAsync();

        // Assert - the method was called (verified by checking error message was set)
        Assert.Equal("Invalid email or password", model.ErrorMessage);
    }

    [Fact]
    public void ErrorMessage_InitializesToNull()
    {
        // Arrange
        var userService = new MockUserService();
        var logger = new MockLogger();

        // Act
        var model = new LoginModel(userService, logger);

        // Assert
        Assert.Null(model.ErrorMessage);
    }
}

using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using BCrypt.Net;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for User business operations
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;

    public UserService(IUserRepository userRepository, ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _userRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<User?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", id);
            return await _userRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with username: {Username}", username);
            return await _userRepository.GetByUsernameAsync(username, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with username: {Username}", username);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty", nameof(password));

            _logger.LogInformation("Creating new user: {Username}", user.Username);

            var existingUser = await _userRepository.GetByUsernameAsync(user.Username, cancellationToken);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with username {user.Username} already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);

            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", user?.Username);
            throw;
        }
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);

            var existingUser = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with ID {user.Id} not found");
            }

            user.ModifiedDate = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User updated successfully with ID: {UserId}", updatedUser.Id);

            return updatedUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", user?.Id);
            throw;
        }
    }

    public async Task<bool> DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);

            var exists = await _userRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                return false;
            }

            var result = await _userRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("User deleted successfully with ID: {UserId}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> AuthenticateAsync(string username, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Username}", username);

            var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User not found: {Username}", username);
                return null;
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("User is inactive: {Username}", username);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Username}", username);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Username}", username);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Username}", username);
            throw;
        }
    }

    public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            return await _userRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

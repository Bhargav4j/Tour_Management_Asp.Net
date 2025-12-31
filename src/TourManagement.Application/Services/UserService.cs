using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using BCrypt.Net;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for User business logic
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

    public async Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Successfully retrieved {Count} users", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with email: {Email} not found", email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfo> CreateUserAsync(UserInfo user, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", user.Email);

            var exists = await _userRepository.ExistsAsync(user.Email, cancellationToken);
            if (exists)
            {
                _logger.LogWarning("User with email: {Email} already exists", user.Email);
                throw new InvalidOperationException($"User with email {user.Email} already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("Successfully created user with email: {Email}", createdUser.Email);

            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email);
            throw;
        }
    }

    public async Task UpdateUserAsync(UserInfo user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email: {Email}", user.Email);

            var existingUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogWarning("User with email: {Email} not found for update", user.Email);
                throw new InvalidOperationException($"User with email {user.Email} not found");
            }

            user.ModifiedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("Successfully updated user with email: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email: {Email}", user.Email);
            throw;
        }
    }

    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email: {Email}", email);

            var exists = await _userRepository.ExistsAsync(email, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("User with email: {Email} not found for deletion", email);
                throw new InvalidOperationException($"User with email {email} not found");
            }

            await _userRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("Successfully deleted user with email: {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userRepository.SearchAsync(searchTerm, cancellationToken);
            _logger.LogInformation("Successfully found {Count} users matching search term", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<UserInfo?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating user: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with email: {Email} not found during validation", email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Email}", email);
                return null;
            }

            _logger.LogInformation("Successfully validated user: {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user: {Email}", email);
            throw;
        }
    }
}

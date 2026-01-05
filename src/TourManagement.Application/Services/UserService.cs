using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

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

    public async Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} users", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            throw;
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
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

    public async Task<User> RegisterUserAsync(User user, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new user: {Email}", user.Email);

            var existingUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email: {Email} already exists", user.Email);
                throw new InvalidOperationException($"User with email {user.Email} already exists");
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User registered successfully: {Email}", createdUser.Email);
            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", user.Email);
            throw;
        }
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user: {Email}", user.Email);

            var existingUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogWarning("User with email: {Email} not found for update", user.Email);
                throw new InvalidOperationException($"User with email {user.Email} not found");
            }

            user.ModifiedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);
            _logger.LogInformation("User updated successfully: {Email}", user.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user: {Email}", user.Email);
            throw;
        }
    }

    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user: {Email}", email);

            var exists = await _userRepository.ExistsAsync(email, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("User with email: {Email} not found for deletion", email);
                throw new InvalidOperationException($"User with email {email} not found");
            }

            await _userRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("User deleted successfully: {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user: {Email}", email);
            throw;
        }
    }

    public async Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null)
            {
                _logger.LogWarning("User with email: {Email} not found during authentication", email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Email}", email);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw;
        }
    }
}

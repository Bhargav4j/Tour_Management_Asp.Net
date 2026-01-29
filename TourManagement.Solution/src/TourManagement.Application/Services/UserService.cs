using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for User operations
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
            _logger.LogError(ex, "Error retrieving all users");
            throw new TourManagementException("Error retrieving users", ex);
        }
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                throw new EntityNotFoundException(nameof(User), email);
            }

            return user;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw new TourManagementException($"Error retrieving user with email {email}", ex);
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
                throw new EntityAlreadyExistsException(nameof(User), user.Email);
            }

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User registered successfully: {Email}", createdUser.Email);

            return createdUser;
        }
        catch (EntityAlreadyExistsException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering user: {Email}", user.Email);
            throw new TourManagementException("Error registering user", ex);
        }
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", user.Email);

            var existingUser = await _userRepository.GetByEmailAsync(user.Email, cancellationToken);
            if (existingUser == null)
            {
                throw new EntityNotFoundException(nameof(User), user.Email);
            }

            user.ModifiedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("User with email {Email} updated successfully", user.Email);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", user.Email);
            throw new TourManagementException($"Error updating user with email {user.Email}", ex);
        }
    }

    public async Task DeleteUserAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);

            var exists = await _userRepository.ExistsAsync(email, cancellationToken);
            if (!exists)
            {
                throw new EntityNotFoundException(nameof(User), email);
            }

            await _userRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("User with email {Email} deleted successfully", email);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw new TourManagementException($"Error deleting user with email {email}", ex);
        }
    }

    public async Task<User?> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);

            var user = await _userRepository.GetByEmailAsync(email, cancellationToken);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Authentication failed for user: {Email}", email);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw new TourManagementException("Error authenticating user", ex);
        }
    }
}

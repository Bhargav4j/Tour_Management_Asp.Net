using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

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
        _userRepository = userRepository;
        _logger = logger;
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

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _userRepository.GetByEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw;
        }
    }

    public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", user.Email);
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            return await _userRepository.AddAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", user.Email);
            throw;
        }
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", user.Id);
            user.ModifiedDate = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", user.Id);
            throw;
        }
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            await _userRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating user credentials for email: {Email}", email);
            return await _userRepository.ValidateCredentialsAsync(email, password, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user credentials for email: {Email}", email);
            throw;
        }
    }
}

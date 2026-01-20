using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IUserInfoRepository userInfoRepository, ILogger<UserInfoService> logger)
    {
        _userInfoRepository = userInfoRepository ?? throw new ArgumentNullException(nameof(userInfoRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userInfoRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Successfully retrieved {Count} users", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfo?> GetUserByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", id);
            var user = await _userInfoRepository.GetByIdAsync(id, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", id);
                return null;
            }

            _logger.LogInformation("Successfully retrieved user with ID {UserId}", id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email {Email}", email);
            var user = await _userInfoRepository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found", email);
                return null;
            }

            _logger.LogInformation("Successfully retrieved user with email {Email}", email);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email {Email}", email);
            throw;
        }
    }

    public async Task<UserInfo> CreateUserAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", userInfo.Email);

            userInfo.CreatedDate = DateTime.UtcNow;
            userInfo.IsActive = true;

            var createdUser = await _userInfoRepository.AddAsync(userInfo, cancellationToken);
            _logger.LogInformation("Successfully created user with ID {UserId}", createdUser.Id);

            return createdUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", userInfo.Email);
            throw;
        }
    }

    public async Task UpdateUserAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID {UserId}", userInfo.Id);

            var existingUser = await _userInfoRepository.GetByIdAsync(userInfo.Id, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", userInfo.Id);
                throw new EntityNotFoundException(nameof(UserInfo), userInfo.Id);
            }

            userInfo.ModifiedDate = DateTime.UtcNow;
            await _userInfoRepository.UpdateAsync(userInfo, cancellationToken);

            _logger.LogInformation("Successfully updated user with ID {UserId}", userInfo.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", userInfo.Id);
            throw;
        }
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID {UserId}", id);

            var existingUser = await _userInfoRepository.GetByIdAsync(id, cancellationToken);
            if (existingUser == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", id);
                throw new EntityNotFoundException(nameof(UserInfo), id);
            }

            await _userInfoRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Successfully deleted user with ID {UserId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userInfoRepository.SearchAsync(searchTerm, cancellationToken);
            _logger.LogInformation("Found {Count} users matching search term", users.Count());
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating credentials for email: {Email}", email);
            var isValid = await _userInfoRepository.ValidateCredentialsAsync(email, password, cancellationToken);
            _logger.LogInformation("Credential validation result for {Email}: {IsValid}", email, isValid);
            return isValid;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating credentials for email: {Email}", email);
            throw;
        }
    }
}

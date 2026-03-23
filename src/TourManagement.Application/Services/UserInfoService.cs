using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUserInfoRepository _repository;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(IUserInfoRepository repository, ILogger<UserInfoService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _repository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _repository.GetByIdAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfo> CreateAsync(UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", userInfo.Email);

            // Hash password before storing
            userInfo.Password = BCrypt.Net.BCrypt.HashPassword(userInfo.Password);
            userInfo.CreatedDate = DateTime.UtcNow;
            userInfo.IsActive = true;

            return await _repository.AddAsync(userInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", userInfo.Email);
            throw;
        }
    }

    public async Task UpdateAsync(string email, UserInfo userInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email: {Email}", email);
            var existingUser = await _repository.GetByIdAsync(email, cancellationToken);
            if (existingUser == null)
            {
                throw new InvalidOperationException($"User with email {email} not found");
            }

            userInfo.Email = email;
            userInfo.ModifiedDate = DateTime.UtcNow;
            userInfo.CreatedDate = existingUser.CreatedDate;
            userInfo.CreatedBy = existingUser.CreatedBy;

            // If password is being updated, hash it
            if (!string.IsNullOrEmpty(userInfo.Password) && userInfo.Password != existingUser.Password)
            {
                userInfo.Password = BCrypt.Net.BCrypt.HashPassword(userInfo.Password);
            }
            else
            {
                userInfo.Password = existingUser.Password;
            }

            await _repository.UpdateAsync(userInfo, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email: {Email}", email);
            throw;
        }
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email: {Email}", email);
            await _repository.DeleteAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Email}", email);
            var user = await _repository.GetByIdAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", email);
                return null;
            }

            // Verify password using BCrypt
            if (BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _logger.LogInformation("User authenticated successfully: {Email}", email);
                return user;
            }

            _logger.LogWarning("Invalid password for user: {Email}", email);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error authenticating user: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            return await _repository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

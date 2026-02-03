using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo operations
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

    public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            return await _userInfoRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            return await _userInfoRepository.GetByEmailAsync(email, cancellationToken);
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

            var exists = await _userInfoRepository.ExistsAsync(userInfo.Email, cancellationToken);
            if (exists)
            {
                throw new ValidationException($"User with email {userInfo.Email} already exists");
            }

            // Hash password
            userInfo.Password = HashPassword(userInfo.Password);
            userInfo.CreatedDate = DateTime.UtcNow;
            userInfo.IsActive = true;
            userInfo.CreatedBy = "System";

            var result = await _userInfoRepository.AddAsync(userInfo, cancellationToken);
            _logger.LogInformation("User created successfully: {Email}", result.Email);
            return result;
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

            var existingUser = await _userInfoRepository.GetByEmailAsync(email, cancellationToken);
            if (existingUser == null)
            {
                throw new NotFoundException(nameof(UserInfo), email);
            }

            existingUser.FirstName = userInfo.FirstName;
            existingUser.LastName = userInfo.LastName;
            existingUser.Gender = userInfo.Gender;
            existingUser.DateOfBirth = userInfo.DateOfBirth;
            existingUser.Street = userInfo.Street;
            existingUser.City = userInfo.City;
            existingUser.State = userInfo.State;
            existingUser.ModifiedDate = DateTime.UtcNow;
            existingUser.ModifiedBy = "System";

            await _userInfoRepository.UpdateAsync(existingUser, cancellationToken);
            _logger.LogInformation("User updated successfully: {Email}", email);
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

            var exists = await _userInfoRepository.ExistsAsync(email, cancellationToken);
            if (!exists)
            {
                throw new NotFoundException(nameof(UserInfo), email);
            }

            await _userInfoRepository.DeleteAsync(email, cancellationToken);
            _logger.LogInformation("User deleted successfully: {Email}", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfo?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating user: {Email}", email);

            var hashedPassword = HashPassword(password);
            var user = await _userInfoRepository.ValidateUserAsync(email, hashedPassword, cancellationToken);

            if (user != null)
            {
                _logger.LogInformation("User validation successful: {Email}", email);
            }
            else
            {
                _logger.LogWarning("User validation failed: {Email}", email);
            }

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            return await _userInfoRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

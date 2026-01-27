using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for UserInfo business operations
/// </summary>
public class UserInfoService : IUserInfoService
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserInfoService> _logger;

    public UserInfoService(
        IUserInfoRepository userInfoRepository,
        IMapper mapper,
        ILogger<UserInfoService> logger)
    {
        _userInfoRepository = userInfoRepository ?? throw new ArgumentNullException(nameof(userInfoRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userInfoRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserInfoDto?> GetByIdAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with email: {Email}", email);
            var user = await _userInfoRepository.GetByIdAsync(email, cancellationToken);
            return user == null ? null : _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfoDto> CreateAsync(UserInfoCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", dto.Email);

            var exists = await _userInfoRepository.ExistsAsync(dto.Email, cancellationToken);
            if (exists)
            {
                throw new InvalidOperationException($"User with email {dto.Email} already exists");
            }

            var user = _mapper.Map<UserInfo>(dto);
            var createdUser = await _userInfoRepository.AddAsync(user, cancellationToken);
            return _mapper.Map<UserInfoDto>(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", dto.Email);
            throw;
        }
    }

    public async Task UpdateAsync(string email, UserInfoUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email: {Email}", email);
            var existingUser = await _userInfoRepository.GetByIdAsync(email, cancellationToken);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found");
            }

            _mapper.Map(dto, existingUser);
            existingUser.ModifiedDate = DateTime.UtcNow;
            await _userInfoRepository.UpdateAsync(existingUser, cancellationToken);
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
                throw new KeyNotFoundException($"User with email {email} not found");
            }
            await _userInfoRepository.DeleteAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email: {Email}", email);
            throw;
        }
    }

    public async Task<UserInfoDto?> ValidateLoginAsync(UserLoginDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for user: {Email}", dto.Email);
            var user = await _userInfoRepository.GetByEmailAndPasswordAsync(dto.Email, dto.Password, cancellationToken);
            return user == null ? null : _mapper.Map<UserInfoDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for user: {Email}", dto.Email);
            throw;
        }
    }

    public async Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userInfoRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserInfoDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

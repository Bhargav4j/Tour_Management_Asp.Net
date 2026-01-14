using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository repository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all users");
            var users = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users");
            throw;
        }
    }

    public async Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting user with email {Email}", email);
            var user = await _repository.GetByEmailAsync(email, cancellationToken);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user with email {Email}", email);
            throw;
        }
    }

    public async Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", dto.Email);
            var user = _mapper.Map<UserInfo>(dto);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var created = await _repository.AddAsync(user, cancellationToken);
            return _mapper.Map<UserDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", dto.Email);
            throw;
        }
    }

    public async Task UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with email {Email}", email);
            var existing = await _repository.GetByEmailAsync(email, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found");
            }

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.UtcNow;
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with email {Email}", email);
            throw;
        }
    }

    public async Task DeleteAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with email {Email}", email);
            await _repository.DeleteAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with email {Email}", email);
            throw;
        }
    }

    public async Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for user: {Email}", email);
            var user = await _repository.GetByEmailAsync(email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}", email);
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Email}", email);
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for user: {Email}", email);
            throw;
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for User business logic
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all users");
            var users = await _userRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all users");
            throw;
        }
    }

    public async Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving user with ID: {UserId}", id);
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            return user != null ? _mapper.Map<UserDto>(user) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<UserDto> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Username}", createDto.Username);
            var user = _mapper.Map<User>(createDto);
            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("User created successfully with ID: {UserId}", createdUser.Id);
            return _mapper.Map<UserDto>(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Username}", createDto.Username);
            throw;
        }
    }

    public async Task UpdateAsync(int id, UserUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating user with ID: {UserId}", id);
            var existingUser = await _userRepository.GetByIdAsync(id, cancellationToken);

            if (existingUser == null)
            {
                _logger.LogWarning("User with ID: {UserId} not found", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            _mapper.Map(updateDto, existingUser);
            await _userRepository.UpdateAsync(existingUser, cancellationToken);
            _logger.LogInformation("User with ID: {UserId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var exists = await _userRepository.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("User with ID: {UserId} not found", id);
                throw new KeyNotFoundException($"User with ID {id} not found");
            }

            await _userRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("User with ID: {UserId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching users with term: {SearchTerm}", searchTerm);
            var users = await _userRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching users with term: {SearchTerm}", searchTerm);
            throw;
        }
    }

    public async Task<UserDto?> ValidateUserAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating user: {Username}", loginDto.Username);
            var user = await _userRepository.ValidateUserAsync(loginDto.Username, loginDto.Password, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for user: {Username}", loginDto.Username);
                return null;
            }

            _logger.LogInformation("User validated successfully: {Username}", loginDto.Username);
            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating user: {Username}", loginDto.Username);
            throw;
        }
    }
}

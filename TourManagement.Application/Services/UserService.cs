using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

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
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
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

    public async Task<UserDto> CreateAsync(UserCreateDto createDto, string createdBy, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new user: {Email}", createDto.Email);

            var existingUser = await _userRepository.GetByEmailAsync(createDto.Email, cancellationToken);
            if (existingUser != null)
            {
                _logger.LogWarning("User with email {Email} already exists", createDto.Email);
                throw new InvalidOperationException($"User with email {createDto.Email} already exists");
            }

            var user = _mapper.Map<User>(createDto);
            user.CreatedBy = createdBy;
            user.CreatedDate = DateTime.UtcNow;
            user.IsActive = true;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);
            _logger.LogInformation("Successfully created user with ID: {UserId}", createdUser.Id);
            return _mapper.Map<UserDto>(createdUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user: {Email}", createDto.Email);
            throw;
        }
    }

    public async Task<UserDto> UpdateAsync(int id, UserUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default)
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
            existingUser.ModifiedBy = modifiedBy;
            existingUser.ModifiedDate = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(existingUser, cancellationToken);
            _logger.LogInformation("Successfully updated user with ID: {UserId}", id);
            return _mapper.Map<UserDto>(updatedUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting user with ID: {UserId}", id);
            var result = await _userRepository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.LogInformation("Successfully deleted user with ID: {UserId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete user with ID: {UserId}", id);
            }
            return result;
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

    public async Task<UserLoginResultDto> LoginAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("User login attempt for email: {Email}", loginDto.Email);
            var user = await _userRepository.GetByEmailAsync(loginDto.Email, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", loginDto.Email);
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for email: {Email}", loginDto.Email);
                return new UserLoginResultDto
                {
                    Success = false,
                    Message = "Invalid email or password"
                };
            }

            _logger.LogInformation("User logged in successfully: {Email}", loginDto.Email);
            return new UserLoginResultDto
            {
                Success = true,
                Message = "Login successful",
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", loginDto.Email);
            throw;
        }
    }
}

using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for User business logic operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UserUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<UserDto?> ValidateUserAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default);
}

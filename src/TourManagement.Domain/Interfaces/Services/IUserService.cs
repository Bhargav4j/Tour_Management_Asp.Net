using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string email, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
}

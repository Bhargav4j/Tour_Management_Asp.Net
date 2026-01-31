using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for User business operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UserDto> CreateAsync(UserCreateDto createDto, string createdBy, CancellationToken cancellationToken = default);

    Task<UserDto> UpdateAsync(int id, UserUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<UserDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

    Task<UserLoginResultDto> LoginAsync(UserLoginDto loginDto, CancellationToken cancellationToken = default);
}

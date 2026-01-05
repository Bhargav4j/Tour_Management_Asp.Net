using TourManagement.Domain.DTOs;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto> RegisterAsync(UserRegisterDto userRegisterDto, CancellationToken cancellationToken = default);
    Task<UserDto?> AuthenticateAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UserUpdateDto userUpdateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

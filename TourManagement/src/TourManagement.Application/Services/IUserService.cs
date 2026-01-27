using TourManagement.Application.DTOs;

namespace TourManagement.Application.Services;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserDto> CreateAsync(UserCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UserUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<UserDto?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}

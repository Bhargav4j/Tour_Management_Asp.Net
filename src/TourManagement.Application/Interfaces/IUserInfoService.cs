using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

public interface IUserInfoService
{
    Task<IEnumerable<UserInfoDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfoDto?> GetByIdAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfoDto> CreateAsync(UserInfoCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(string email, UserInfoUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfoDto?> ValidateLoginAsync(UserLoginDto dto, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfoDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

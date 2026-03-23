using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo business operations
/// </summary>
public interface IUserInfoService
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> CreateAsync(UserInfo userInfo, CancellationToken cancellationToken = default);
    Task UpdateAsync(string email, UserInfo userInfo, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

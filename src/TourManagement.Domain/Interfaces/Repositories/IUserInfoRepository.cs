using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity operations
/// </summary>
public interface IUserInfoRepository
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByIdAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> AddAsync(UserInfo userInfo, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserInfo userInfo, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByEmailAndPasswordAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

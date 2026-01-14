using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for UserInfo entity
/// </summary>
public interface IUserRepository
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> AddAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task UpdateAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo?> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}

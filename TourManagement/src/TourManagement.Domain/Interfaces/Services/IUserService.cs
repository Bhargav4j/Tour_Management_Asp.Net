using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for UserInfo operations with DTOs
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UserInfo?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> CreateAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task<UserInfo> UpdateAsync(int id, UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<bool> ValidateCredentialsAsync(string email, string password, CancellationToken cancellationToken = default);
}

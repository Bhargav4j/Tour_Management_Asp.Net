using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User business logic operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<UserInfo>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<UserInfo?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UserInfo> CreateUserAsync(UserInfo user, string password, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(UserInfo user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserInfo>> SearchUsersAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<UserInfo?> ValidateUserAsync(string email, string password, CancellationToken cancellationToken = default);
}

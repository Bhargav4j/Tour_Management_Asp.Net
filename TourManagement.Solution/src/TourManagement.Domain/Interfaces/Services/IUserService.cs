using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for User operations
/// </summary>
public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync(CancellationToken cancellationToken = default);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> RegisterUserAsync(User user, string password, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken = default);
}

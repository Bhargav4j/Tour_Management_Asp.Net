using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity
/// </summary>
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}

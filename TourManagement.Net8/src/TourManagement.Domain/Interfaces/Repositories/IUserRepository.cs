using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for User entity operations
/// </summary>
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User> AddAsync(User user, CancellationToken cancellationToken = default);
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<User?> ValidateUserAsync(string username, string password, CancellationToken cancellationToken = default);
}

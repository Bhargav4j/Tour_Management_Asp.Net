using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for Admin entity operations
/// </summary>
public interface IAdminRepository
{
    Task<Admin?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<Admin?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Admin> AddAsync(Admin admin, CancellationToken cancellationToken = default);
    Task<bool> ValidateCredentialsAsync(string username, string password, CancellationToken cancellationToken = default);
}

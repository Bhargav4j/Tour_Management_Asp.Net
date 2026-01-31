using TourManagement.Application.DTOs;

namespace TourManagement.Application.Contracts.Services;

public interface IAdminService
{
    Task<bool> ValidateLoginAsync(string username, string password, CancellationToken cancellationToken = default);
    Task<AdminDto?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
}

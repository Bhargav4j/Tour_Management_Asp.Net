using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Tour operations with DTOs
/// </summary>
public interface ITourService
{
    Task<IEnumerable<Tour>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tour?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tour> CreateAsync(Tour tour, CancellationToken cancellationToken = default);
    Task<Tour> UpdateAsync(int id, Tour tour, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Tour>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

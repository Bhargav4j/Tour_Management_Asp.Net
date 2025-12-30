using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for Tour business operations
/// </summary>
public interface ITourService
{
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(TourCreateDto createDto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TourUpdateDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<TourDto>> GetByPlaceAsync(string place, CancellationToken cancellationToken = default);
}

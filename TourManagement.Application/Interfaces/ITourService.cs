using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for Tour business operations
/// </summary>
public interface ITourService
{
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<TourDto> CreateAsync(TourCreateDto createDto, string createdBy, CancellationToken cancellationToken = default);

    Task<TourDto> UpdateAsync(int id, TourUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

public interface ITourService
{
    Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}

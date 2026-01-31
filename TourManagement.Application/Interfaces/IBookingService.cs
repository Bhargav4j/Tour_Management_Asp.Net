using TourManagement.Application.DTOs;

namespace TourManagement.Application.Interfaces;

/// <summary>
/// Service interface for Booking business operations
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IEnumerable<BookingDto>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default);

    Task<BookingDto> CreateAsync(BookingCreateDto createDto, string createdBy, CancellationToken cancellationToken = default);

    Task<BookingDto> UpdateAsync(int id, BookingUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

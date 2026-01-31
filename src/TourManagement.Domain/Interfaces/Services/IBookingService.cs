using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for Booking business operations
/// </summary>
public interface IBookingService
{
    Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default);
    Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(int tourId, CancellationToken cancellationToken = default);
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<Booking> UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<bool> DeleteBookingAsync(int id, CancellationToken cancellationToken = default);
}

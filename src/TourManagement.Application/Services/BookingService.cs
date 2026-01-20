using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository bookingRepository, ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
            _logger.LogInformation("Successfully retrieved {Count} bookings", bookings.Count());
            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    public async Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID {BookingId}", id);
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);

            if (booking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                return null;
            }

            _logger.LogInformation("Successfully retrieved booking with ID {BookingId}", id);
            return booking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user with ID {UserId}", userId);
            var bookings = await _bookingRepository.GetByUserIdAsync(userId, cancellationToken);
            _logger.LogInformation("Successfully retrieved {Count} bookings for user {UserId}", bookings.Count(), userId);
            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user with ID {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for tour with ID {TourId}", tourId);
            var bookings = await _bookingRepository.GetByTourIdAsync(tourId, cancellationToken);
            _logger.LogInformation("Successfully retrieved {Count} bookings for tour {TourId}", bookings.Count(), tourId);
            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for tour with ID {TourId}", tourId);
            throw;
        }
    }

    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user {UserId} and tour {TourId}", booking.UserId, booking.TourId);

            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            booking.BookingDate = DateTime.UtcNow;
            booking.Status = "Pending";

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Successfully created booking with ID {BookingId}", createdBooking.Id);

            return createdBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {UserId} and tour {TourId}", booking.UserId, booking.TourId);
            throw;
        }
    }

    public async Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID {BookingId}", booking.Id);

            var existingBooking = await _bookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found for update", booking.Id);
                throw new EntityNotFoundException(nameof(Booking), booking.Id);
            }

            booking.ModifiedDate = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            _logger.LogInformation("Successfully updated booking with ID {BookingId}", booking.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", booking.Id);
            throw;
        }
    }

    public async Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);

            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found for deletion", id);
                throw new EntityNotFoundException(nameof(Booking), id);
            }

            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Successfully deleted booking with ID {BookingId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw;
        }
    }
}

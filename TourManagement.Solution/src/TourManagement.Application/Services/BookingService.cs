using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking operations
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
            _logger.LogInformation("Retrieved {Count} bookings", bookings.Count());
            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw new TourManagementException("Error retrieving bookings", ex);
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
                throw new EntityNotFoundException(nameof(Booking), id);
            }

            return booking;
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID {BookingId}", id);
            throw new TourManagementException($"Error retrieving booking with ID {id}", ex);
        }
    }

    public async Task<IEnumerable<Booking>> GetUserBookingsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user: {Email}", email);
            var bookings = await _bookingRepository.GetByUserEmailAsync(email, cancellationToken);
            _logger.LogInformation("Retrieved {Count} bookings for user: {Email}", bookings.Count(), email);
            return bookings;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user: {Email}", email);
            throw new TourManagementException($"Error retrieving bookings for user {email}", ex);
        }
    }

    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user: {Email}", booking.UserEmail);

            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID {BookingId}", createdBooking.Id);

            return createdBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user: {Email}", booking.UserEmail);
            throw new TourManagementException("Error creating booking", ex);
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
                throw new EntityNotFoundException(nameof(Booking), booking.Id);
            }

            booking.ModifiedDate = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            _logger.LogInformation("Booking with ID {BookingId} updated successfully", booking.Id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID {BookingId}", booking.Id);
            throw new TourManagementException($"Error updating booking with ID {booking.Id}", ex);
        }
    }

    public async Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID {BookingId}", id);

            var exists = await _bookingRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new EntityNotFoundException(nameof(Booking), id);
            }

            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking with ID {BookingId} deleted successfully", id);
        }
        catch (EntityNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID {BookingId}", id);
            throw new TourManagementException($"Error deleting booking with ID {id}", ex);
        }
    }
}

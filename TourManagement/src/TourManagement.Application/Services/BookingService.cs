using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking operations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        ITourRepository tourRepository,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _tourRepository = tourRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all bookings");
            return await _bookingRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all bookings");
            throw;
        }
    }

    public async Task<Booking?> GetBookingByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting booking with ID: {BookingId}", id);
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bookings for user ID: {UserId}", userId);
            return await _bookingRepository.GetByUserIdAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bookings for tour ID: {TourId}", tourId);
            return await _bookingRepository.GetByTourIdAsync(tourId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for tour ID: {TourId}", tourId);
            throw;
        }
    }

    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour ID: {TourId}", booking.TourId);

            var tour = await _tourRepository.GetByIdAsync(booking.TourId, cancellationToken);
            if (tour == null)
            {
                throw new InvalidOperationException($"Tour with ID {booking.TourId} not found");
            }

            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            booking.TotalAmount = tour.Price * booking.NumberOfPeople;

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID: {BookingId}", createdBooking.Id);

            return createdBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour ID: {TourId}", booking.TourId);
            throw;
        }
    }

    public async Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", booking.Id);

            var existingBooking = await _bookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking == null)
            {
                throw new InvalidOperationException($"Booking with ID {booking.Id} not found");
            }

            booking.ModifiedDate = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(booking, cancellationToken);

            _logger.LogInformation("Booking updated successfully: {BookingId}", booking.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", booking.Id);
            throw;
        }
    }

    public async Task DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID: {BookingId}", id);

            var exists = await _bookingRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new InvalidOperationException($"Booking with ID {id} not found");
            }

            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking deleted successfully: {BookingId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }
}

using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations
/// </summary>
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        ITourRepository tourRepository,
        IUserRepository userRepository,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            return await _bookingRepository.GetAllAsync(cancellationToken);
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
            _logger.LogInformation("Retrieving booking with ID: {BookingId}", id);
            return await _bookingRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user ID: {UserId}", userId);
            return await _bookingRepository.GetByUserIdAsync(userId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user ID: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetBookingsByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for tour ID: {TourId}", tourId);
            return await _bookingRepository.GetByTourIdAsync(tourId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for tour ID: {TourId}", tourId);
            throw;
        }
    }

    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            _logger.LogInformation("Creating new booking for tour ID: {TourId}, user ID: {UserId}", booking.TourId, booking.UserId);

            var tour = await _tourRepository.GetByIdAsync(booking.TourId, cancellationToken);
            if (tour == null)
            {
                throw new InvalidOperationException($"Tour with ID {booking.TourId} not found");
            }

            var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);
            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {booking.UserId} not found");
            }

            booking.TotalAmount = tour.Price * booking.NumberOfPeople;
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID: {BookingId}", createdBooking.Id);

            return createdBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking");
            throw;
        }
    }

    public async Task<Booking> UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            if (booking == null)
                throw new ArgumentNullException(nameof(booking));

            _logger.LogInformation("Updating booking with ID: {BookingId}", booking.Id);

            var existingBooking = await _bookingRepository.GetByIdAsync(booking.Id, cancellationToken);
            if (existingBooking == null)
            {
                throw new InvalidOperationException($"Booking with ID {booking.Id} not found");
            }

            booking.ModifiedDate = DateTime.UtcNow;

            var updatedBooking = await _bookingRepository.UpdateAsync(booking, cancellationToken);
            _logger.LogInformation("Booking updated successfully with ID: {BookingId}", updatedBooking.Id);

            return updatedBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", booking?.Id);
            throw;
        }
    }

    public async Task<bool> DeleteBookingAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID: {BookingId}", id);

            var exists = await _bookingRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found for deletion", id);
                return false;
            }

            var result = await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking deleted successfully with ID: {BookingId}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }
}

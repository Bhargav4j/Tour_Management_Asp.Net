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
    private readonly IUserRepository _userRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        ITourRepository tourRepository,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
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

    public async Task<Booking?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<Booking>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
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

    public async Task<IEnumerable<Booking>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
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

    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user ID: {UserId}, tour ID: {TourId}", booking.UserId, booking.TourId);

            if (!await _userRepository.ExistsAsync(booking.UserId, cancellationToken))
            {
                _logger.LogWarning("User with ID {UserId} not found", booking.UserId);
                throw new InvalidOperationException($"User with ID {booking.UserId} not found");
            }

            if (!await _tourRepository.ExistsAsync(booking.TourId, cancellationToken))
            {
                _logger.LogWarning("Tour with ID {TourId} not found", booking.TourId);
                throw new InvalidOperationException($"Tour with ID {booking.TourId} not found");
            }

            booking.BookingDate = DateTime.UtcNow;
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            booking.CreatedBy = "System";
            booking.Status = "Confirmed";

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Booking created successfully with ID: {BookingId}", createdBooking.Id);
            return createdBooking;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user ID: {UserId}, tour ID: {TourId}", booking.UserId, booking.TourId);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", id);

            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                throw new InvalidOperationException($"Booking with ID {id} not found");
            }

            existingBooking.NumberOfPeople = booking.NumberOfPeople;
            existingBooking.TotalAmount = booking.TotalAmount;
            existingBooking.Status = booking.Status;
            existingBooking.ModifiedDate = DateTime.UtcNow;
            existingBooking.ModifiedBy = "System";

            await _bookingRepository.UpdateAsync(existingBooking, cancellationToken);
            _logger.LogInformation("Booking updated successfully with ID: {BookingId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID: {BookingId}", id);

            if (!await _bookingRepository.ExistsAsync(id, cancellationToken))
            {
                _logger.LogWarning("Booking with ID {BookingId} not found", id);
                throw new InvalidOperationException($"Booking with ID {id} not found");
            }

            await _bookingRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Booking deleted successfully with ID: {BookingId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }
}

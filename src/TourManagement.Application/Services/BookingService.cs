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
    private readonly IBookingRepository _repository;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository repository, ILogger<BookingService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Booking>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            return await _repository.GetAllAsync(cancellationToken);
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
            return await _repository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<Booking> CreateAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user: {Email}", booking.Email);
            booking.CreatedDate = DateTime.UtcNow;
            booking.IsActive = true;
            return await _repository.AddAsync(booking, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user: {Email}", booking.Email);
            throw;
        }
    }

    public async Task UpdateAsync(int id, Booking booking, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", id);
            var existingBooking = await _repository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                throw new InvalidOperationException($"Booking with ID {id} not found");
            }

            booking.BookingId = id;
            booking.ModifiedDate = DateTime.UtcNow;
            booking.CreatedDate = existingBooking.CreatedDate;
            booking.CreatedBy = existingBooking.CreatedBy;
            await _repository.UpdateAsync(booking, cancellationToken);
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
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user: {Email}", email);
            return await _repository.GetByUserEmailAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<Booking>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            return await _repository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

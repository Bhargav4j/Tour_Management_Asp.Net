using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository bookingRepository,
        IMapper mapper,
        ILogger<BookingService> logger)
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all bookings");
            var bookings = await _bookingRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all bookings");
            throw;
        }
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving booking with ID: {BookingId}", id);
            var booking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            return booking != null ? _mapper.Map<BookingDto>(booking) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user email: {Email}", email);
            var bookings = await _bookingRepository.GetByUserEmailAsync(email, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user email: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for tour ID: {TourId}", tourId);
            var bookings = await _bookingRepository.GetByTourIdAsync(tourId, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for tour ID: {TourId}", tourId);
            throw;
        }
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto createDto, string createdBy, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for tour: {TourId}", createDto.TourId);
            var booking = _mapper.Map<Booking>(createDto);
            booking.CreatedBy = createdBy;
            booking.CreatedDate = DateTime.UtcNow;
            booking.BookingDate = DateTime.UtcNow;
            booking.IsActive = true;

            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            _logger.LogInformation("Successfully created booking with ID: {BookingId}", createdBooking.Id);
            return _mapper.Map<BookingDto>(createdBooking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for tour: {TourId}", createDto.TourId);
            throw;
        }
    }

    public async Task<BookingDto> UpdateAsync(int id, BookingUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", id);
            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                _logger.LogWarning("Booking with ID: {BookingId} not found", id);
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            _mapper.Map(updateDto, existingBooking);
            existingBooking.ModifiedBy = modifiedBy;
            existingBooking.ModifiedDate = DateTime.UtcNow;

            var updatedBooking = await _bookingRepository.UpdateAsync(existingBooking, cancellationToken);
            _logger.LogInformation("Successfully updated booking with ID: {BookingId}", id);
            return _mapper.Map<BookingDto>(updatedBooking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with ID: {BookingId}", id);
            var result = await _bookingRepository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.LogInformation("Successfully deleted booking with ID: {BookingId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete booking with ID: {BookingId}", id);
            }
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }
}

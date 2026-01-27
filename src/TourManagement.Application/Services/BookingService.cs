using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Booking business operations
/// </summary>
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
        _bookingRepository = bookingRepository ?? throw new ArgumentNullException(nameof(bookingRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user: {Email}", dto.Email);
            var booking = _mapper.Map<Booking>(dto);
            var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);
            return _mapper.Map<BookingDto>(createdBooking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user: {Email}", dto.Email);
            throw;
        }
    }

    public async Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with ID: {BookingId}", id);
            var existingBooking = await _bookingRepository.GetByIdAsync(id, cancellationToken);
            if (existingBooking == null)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }

            _mapper.Map(dto, existingBooking);
            existingBooking.ModifiedDate = DateTime.UtcNow;
            await _bookingRepository.UpdateAsync(existingBooking, cancellationToken);
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
            var exists = await _bookingRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                throw new KeyNotFoundException($"Booking with ID {id} not found");
            }
            await _bookingRepository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with ID: {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bookings for user: {Email}", email);
            var bookings = await _bookingRepository.GetByUserEmailAsync(email, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bookings for user: {Email}", email);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching bookings with term: {SearchTerm}", searchTerm);
            var bookings = await _bookingRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching bookings with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

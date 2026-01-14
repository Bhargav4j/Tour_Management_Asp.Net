using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IBookingRepository repository,
        IMapper mapper,
        ILogger<BookingService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all bookings");
            var bookings = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all bookings");
            throw;
        }
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting booking with id {BookingId}", id);
            var booking = await _repository.GetByIdAsync(id, cancellationToken);
            return booking == null ? null : _mapper.Map<BookingDto>(booking);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting booking with id {BookingId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<BookingDto>> GetByUserEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting bookings for user {Email}", email);
            var bookings = await _repository.GetByUserEmailAsync(email, cancellationToken);
            return _mapper.Map<IEnumerable<BookingDto>>(bookings);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting bookings for user {Email}", email);
            throw;
        }
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new booking for user {Email}", dto.Email);
            var booking = _mapper.Map<Booking>(dto);
            var created = await _repository.AddAsync(booking, cancellationToken);
            return _mapper.Map<BookingDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating booking for user {Email}", dto.Email);
            throw;
        }
    }

    public async Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating booking with id {BookingId}", id);
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Booking with id {id} not found");
            }

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.UtcNow;
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating booking with id {BookingId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting booking with id {BookingId}", id);
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting booking with id {BookingId}", id);
            throw;
        }
    }
}

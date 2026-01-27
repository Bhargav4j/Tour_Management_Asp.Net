using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IBookingRepository repository, IMapper mapper, ILogger<BookingService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<BookingDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var bookings = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<BookingDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var booking = await _repository.GetByIdAsync(id, cancellationToken);
        return booking == null ? null : _mapper.Map<BookingDto>(booking);
    }

    public async Task<IEnumerable<BookingDto>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var bookings = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<IEnumerable<BookingDto>> GetByTourIdAsync(int tourId, CancellationToken cancellationToken = default)
    {
        var bookings = await _repository.GetByTourIdAsync(tourId, cancellationToken);
        return _mapper.Map<IEnumerable<BookingDto>>(bookings);
    }

    public async Task<BookingDto> CreateAsync(BookingCreateDto dto, CancellationToken cancellationToken = default)
    {
        var booking = _mapper.Map<Booking>(dto);
        var createdBooking = await _repository.AddAsync(booking, cancellationToken);
        return _mapper.Map<BookingDto>(createdBooking);
    }

    public async Task UpdateAsync(int id, BookingUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingBooking = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingBooking == null) throw new InvalidOperationException($"Booking with ID {id} not found");
        _mapper.Map(dto, existingBooking);
        existingBooking.ModifiedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(existingBooking, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }
}

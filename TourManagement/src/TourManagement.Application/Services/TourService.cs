using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;

namespace TourManagement.Application.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository repository, IMapper mapper, ILogger<TourService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tours = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }

    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tour = await _repository.GetByIdAsync(id, cancellationToken);
        return tour == null ? null : _mapper.Map<TourDto>(tour);
    }

    public async Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        var tour = _mapper.Map<Tour>(dto);
        var createdTour = await _repository.AddAsync(tour, cancellationToken);
        return _mapper.Map<TourDto>(createdTour);
    }

    public async Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existingTour = await _repository.GetByIdAsync(id, cancellationToken);
        if (existingTour == null) throw new InvalidOperationException($"Tour with ID {id} not found");
        _mapper.Map(dto, existingTour);
        existingTour.ModifiedDate = DateTime.UtcNow;
        await _repository.UpdateAsync(existingTour, cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var tours = await _repository.SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IEnumerable<TourDto>>(tours);
    }
}

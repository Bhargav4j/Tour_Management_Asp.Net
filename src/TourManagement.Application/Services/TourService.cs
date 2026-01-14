using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    public TourService(
        ITourRepository repository,
        IMapper mapper,
        ILogger<TourService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting all tours");
            var tours = await _repository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tours");
            throw;
        }
    }

    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Getting tour with id {TourId}", id);
            var tour = await _repository.GetByIdAsync(id, cancellationToken);
            return tour == null ? null : _mapper.Map<TourDto>(tour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting tour with id {TourId}", id);
            throw;
        }
    }

    public async Task<TourDto> CreateAsync(TourCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", dto.TourName);
            var tour = _mapper.Map<Tour>(dto);
            var created = await _repository.AddAsync(tour, cancellationToken);
            return _mapper.Map<TourDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", dto.TourName);
            throw;
        }
    }

    public async Task UpdateAsync(int id, TourUpdateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with id {TourId}", id);
            var existing = await _repository.GetByIdAsync(id, cancellationToken);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Tour with id {id} not found");
            }

            _mapper.Map(dto, existing);
            existing.ModifiedDate = DateTime.UtcNow;
            await _repository.UpdateAsync(existing, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with id {TourId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with id {TourId}", id);
            await _repository.DeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with id {TourId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            var tours = await _repository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

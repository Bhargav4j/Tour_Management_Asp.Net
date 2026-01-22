using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour business logic
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<TourService> _logger;

    public TourService(
        ITourRepository tourRepository,
        IMapper mapper,
        ILogger<TourService> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<TourDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            var tours = await _tourRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public async Task<TourDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID: {TourId}", id);
            var tour = await _tourRepository.GetByIdAsync(id, cancellationToken);
            return tour != null ? _mapper.Map<TourDto>(tour) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task<TourDto> CreateAsync(TourCreateDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", createDto.TourName);
            var tour = _mapper.Map<Tour>(createDto);
            var createdTour = await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created successfully with ID: {TourId}", createdTour.Id);
            return _mapper.Map<TourDto>(createdTour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", createDto.TourName);
            throw;
        }
    }

    public async Task UpdateAsync(int id, TourUpdateDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating tour with ID: {TourId}", id);
            var existingTour = await _tourRepository.GetByIdAsync(id, cancellationToken);

            if (existingTour == null)
            {
                _logger.LogWarning("Tour with ID: {TourId} not found", id);
                throw new KeyNotFoundException($"Tour with ID {id} not found");
            }

            _mapper.Map(updateDto, existingTour);
            await _tourRepository.UpdateAsync(existingTour, cancellationToken);
            _logger.LogInformation("Tour with ID: {TourId} updated successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID: {TourId}", id);
            var exists = await _tourRepository.ExistsAsync(id, cancellationToken);

            if (!exists)
            {
                _logger.LogWarning("Tour with ID: {TourId} not found", id);
                throw new KeyNotFoundException($"Tour with ID {id} not found");
            }

            await _tourRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Tour with ID: {TourId} deleted successfully", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<TourDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            var tours = await _tourRepository.SearchAsync(searchTerm, cancellationToken);
            return _mapper.Map<IEnumerable<TourDto>>(tours);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

using AutoMapper;
using Microsoft.Extensions.Logging;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Application.Interfaces;

namespace TourManagement.Application.Services;

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
        _tourRepository = tourRepository;
        _mapper = mapper;
        _logger = logger;
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

    public async Task<TourDto> CreateAsync(TourCreateDto createDto, string createdBy, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating new tour: {TourName}", createDto.TourName);
            var tour = _mapper.Map<Tour>(createDto);
            tour.CreatedBy = createdBy;
            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;

            var createdTour = await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Successfully created tour with ID: {TourId}", createdTour.Id);
            return _mapper.Map<TourDto>(createdTour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", createDto.TourName);
            throw;
        }
    }

    public async Task<TourDto> UpdateAsync(int id, TourUpdateDto updateDto, string modifiedBy, CancellationToken cancellationToken = default)
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
            existingTour.ModifiedBy = modifiedBy;
            existingTour.ModifiedDate = DateTime.UtcNow;

            var updatedTour = await _tourRepository.UpdateAsync(existingTour, cancellationToken);
            _logger.LogInformation("Successfully updated tour with ID: {TourId}", id);
            return _mapper.Map<TourDto>(updatedTour);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID: {TourId}", id);
            var result = await _tourRepository.DeleteAsync(id, cancellationToken);
            if (result)
            {
                _logger.LogInformation("Successfully deleted tour with ID: {TourId}", id);
            }
            else
            {
                _logger.LogWarning("Failed to delete tour with ID: {TourId}", id);
            }
            return result;
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

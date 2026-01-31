using Microsoft.Extensions.Logging;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Repositories;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Application.Services;

/// <summary>
/// Service implementation for Tour business operations
/// </summary>
public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly ILogger<TourService> _logger;

    public TourService(ITourRepository tourRepository, ILogger<TourService> logger)
    {
        _tourRepository = tourRepository ?? throw new ArgumentNullException(nameof(tourRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<Tour>> GetAllToursAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all tours");
            return await _tourRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public async Task<Tour?> GetTourByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving tour with ID: {TourId}", id);
            var tour = await _tourRepository.GetByIdAsync(id, cancellationToken);
            if (tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
            }
            return tour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task<Tour> CreateTourAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            _logger.LogInformation("Creating new tour: {TourName}", tour.TourName);

            tour.CreatedDate = DateTime.UtcNow;
            tour.IsActive = true;

            var createdTour = await _tourRepository.AddAsync(tour, cancellationToken);
            _logger.LogInformation("Tour created successfully with ID: {TourId}", createdTour.Id);

            return createdTour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour: {TourName}", tour?.TourName);
            throw;
        }
    }

    public async Task<Tour> UpdateTourAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        try
        {
            if (tour == null)
                throw new ArgumentNullException(nameof(tour));

            _logger.LogInformation("Updating tour with ID: {TourId}", tour.Id);

            var existingTour = await _tourRepository.GetByIdAsync(tour.Id, cancellationToken);
            if (existingTour == null)
            {
                throw new InvalidOperationException($"Tour with ID {tour.Id} not found");
            }

            tour.ModifiedDate = DateTime.UtcNow;

            var updatedTour = await _tourRepository.UpdateAsync(tour, cancellationToken);
            _logger.LogInformation("Tour updated successfully with ID: {TourId}", updatedTour.Id);

            return updatedTour;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", tour?.Id);
            throw;
        }
    }

    public async Task<bool> DeleteTourAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting tour with ID: {TourId}", id);

            var exists = await _tourRepository.ExistsAsync(id, cancellationToken);
            if (!exists)
            {
                _logger.LogWarning("Tour with ID {TourId} not found for deletion", id);
                return false;
            }

            var result = await _tourRepository.DeleteAsync(id, cancellationToken);
            _logger.LogInformation("Tour deleted successfully with ID: {TourId}", id);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            throw;
        }
    }

    public async Task<IEnumerable<Tour>> SearchToursAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Searching tours with term: {SearchTerm}", searchTerm);
            return await _tourRepository.SearchAsync(searchTerm, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching tours with term: {SearchTerm}", searchTerm);
            throw;
        }
    }
}

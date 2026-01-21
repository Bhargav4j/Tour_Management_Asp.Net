using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for editing an existing tour
/// </summary>
public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<EditModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public TourViewModel Tour { get; set; } = new TourViewModel();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var tour = await _tourService.GetByIdAsync(id, cancellationToken);
            if (tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToPage("./Index");
            }

            // Map Entity to ViewModel
            Tour = new TourViewModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                PictureFileName = tour.PictureFileName,
                IsActive = tour.IsActive,
                CreatedDate = tour.CreatedDate,
                CreatedBy = tour.CreatedBy,
                ModifiedDate = tour.ModifiedDate,
                ModifiedBy = tour.ModifiedBy
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while retrieving the tour. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(bool deleteImage = false, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var existingTour = await _tourService.GetByIdAsync(Tour.Id, cancellationToken);
            if (existingTour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", Tour.Id);
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToPage("./Index");
            }

            // Handle image operations
            string? fileName = existingTour.PictureFileName;

            // Delete current image if requested
            if (deleteImage && !string.IsNullOrEmpty(fileName))
            {
                DeleteImage(fileName);
                fileName = null;
            }

            // Upload new image if provided
            if (Tour.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(fileName))
                {
                    DeleteImage(fileName);
                }
                fileName = await SaveImageAsync(Tour.ImageFile, cancellationToken);
            }

            // Map ViewModel to Entity
            var tourEntity = new Tour
            {
                Id = Tour.Id,
                TourName = Tour.TourName,
                Place = Tour.Place,
                Days = Tour.Days,
                Price = Tour.Price,
                Locations = Tour.Locations ?? string.Empty,
                TourInfo = Tour.TourInfo ?? string.Empty,
                PictureFileName = fileName,
                IsActive = Tour.IsActive,
                CreatedDate = existingTour.CreatedDate,
                CreatedBy = existingTour.CreatedBy,
                ModifiedDate = DateTime.UtcNow,
                ModifiedBy = User.Identity?.Name ?? "System"
            };

            await _tourService.UpdateAsync(Tour.Id, tourEntity, cancellationToken);

            _logger.LogInformation("Tour updated successfully with ID: {TourId}", Tour.Id);
            TempData["SuccessMessage"] = $"Tour '{Tour.TourName}' updated successfully!";

            return RedirectToPage("./Index");
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Tour with ID {TourId} not found during update", Tour.Id);
            TempData["ErrorMessage"] = "Tour not found.";
            return RedirectToPage("./Index");
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error while updating tour");
            ModelState.AddModelError(string.Empty, ex.Message);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", Tour.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour. Please try again.");
            return Page();
        }
    }

    /// <summary>
    /// Saves the uploaded image to the wwwroot/images/tours directory
    /// </summary>
    private async Task<string?> SaveImageAsync(IFormFile imageFile, CancellationToken cancellationToken)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return null;
        }

        try
        {
            // Validate file size (5MB max)
            if (imageFile.Length > 5242880)
            {
                ModelState.AddModelError("Tour.ImageFile", "File size must be less than 5MB");
                return null;
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Tour.ImageFile", "Invalid file type. Only JPG, PNG, and GIF files are allowed.");
                return null;
            }

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");

            // Ensure directory exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream, cancellationToken);
            }

            _logger.LogInformation("Image saved successfully: {FileName}", uniqueFileName);
            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving image file");
            ModelState.AddModelError("Tour.ImageFile", "An error occurred while saving the image. Please try again.");
            return null;
        }
    }

    /// <summary>
    /// Deletes an image file from the wwwroot/images/tours directory
    /// </summary>
    private void DeleteImage(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return;
        }

        try
        {
            var filePath = Path.Combine(_environment.WebRootPath, "images", "tours", fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                _logger.LogInformation("Image deleted successfully: {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image file: {FileName}", fileName);
            // Don't throw - image deletion failure shouldn't prevent tour update
        }
    }
}

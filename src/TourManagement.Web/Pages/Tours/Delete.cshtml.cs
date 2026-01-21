using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Exceptions;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for deleting a tour
/// </summary>
public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<DeleteModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Loading delete confirmation page for tour with ID: {TourId}", id);
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToPage("./Index");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading delete page for tour with ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the tour. Please try again.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken = default)
    {
        if (Tour == null || Tour.Id == 0)
        {
            _logger.LogWarning("Invalid tour data submitted for deletion");
            TempData["ErrorMessage"] = "Invalid tour data.";
            return RedirectToPage("./Index");
        }

        try
        {
            // Get the tour to retrieve image filename before deletion
            var tourToDelete = await _tourService.GetByIdAsync(Tour.Id, cancellationToken);
            if (tourToDelete == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found during deletion", Tour.Id);
                TempData["ErrorMessage"] = "Tour not found.";
                return RedirectToPage("./Index");
            }

            string? imageFileName = tourToDelete.PictureFileName;
            string tourName = tourToDelete.TourName;

            // Delete the tour from database
            await _tourService.DeleteAsync(Tour.Id, cancellationToken);

            // Delete associated image file if it exists
            if (!string.IsNullOrEmpty(imageFileName))
            {
                DeleteImage(imageFileName);
            }

            _logger.LogInformation("Tour deleted successfully with ID: {TourId}", Tour.Id);
            TempData["SuccessMessage"] = $"Tour '{tourName}' has been deleted successfully.";

            return RedirectToPage("./Index");
        }
        catch (NotFoundException)
        {
            _logger.LogWarning("Tour with ID {TourId} not found during deletion", Tour.Id);
            TempData["ErrorMessage"] = "Tour not found.";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", Tour.Id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour. It may have active bookings or other dependencies. Please contact support if the issue persists.";
            return RedirectToPage("./Index");
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
            else
            {
                _logger.LogWarning("Image file not found: {FileName}", fileName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image file: {FileName}", fileName);
            // Don't throw - image deletion failure shouldn't prevent tour deletion
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ITourService tourService, IWebHostEnvironment environment, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Delete confirmation page loaded for tour ID: {TourId}", id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion, ID: {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            if (!string.IsNullOrEmpty(Tour.PicturePath))
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                var filePath = Path.Combine(uploadsFolder, Tour.PicturePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _tourService.DeleteAsync(id);

            _logger.LogInformation("Tour {TourId} deleted successfully", id);

            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("/Tours/Index");
        }
    }
}

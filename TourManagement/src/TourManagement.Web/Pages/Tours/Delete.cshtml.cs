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

    public DeleteModel(
        ITourService tourService,
        IWebHostEnvironment environment,
        ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id, CancellationToken cancellationToken)
    {
        if (id == null)
        {
            return NotFound();
        }

        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            Tour = await _tourService.GetTourByIdAsync(id.Value, cancellationToken);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion");
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Tour?.Id == null)
        {
            return NotFound();
        }

        if (HttpContext.Session.GetString("IsAdmin") != "true")
        {
            return RedirectToPage("/Users/Login");
        }

        try
        {
            var existingTour = await _tourService.GetTourByIdAsync(Tour.Id, cancellationToken);
            if (existingTour != null && !string.IsNullOrEmpty(existingTour.ImageFileName))
            {
                var imagePath = Path.Combine(_environment.WebRootPath, "images", "tours", existingTour.ImageFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            await _tourService.DeleteTourAsync(Tour.Id, cancellationToken);

            _logger.LogInformation("Tour deleted: {TourId}", Tour.Id);
            TempData["SuccessMessage"] = "Tour deleted successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour");
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the tour.");
            return Page();
        }
    }
}

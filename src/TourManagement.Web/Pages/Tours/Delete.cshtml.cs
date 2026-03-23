using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [BindProperty]
    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while loading the tour.";
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (Tour == null || Tour.TourId == 0)
        {
            return NotFound();
        }

        try
        {
            await _tourService.DeleteAsync(Tour.TourId, cancellationToken);

            _logger.LogInformation("Deleted tour: {TourId}", Tour.TourId);
            TempData["SuccessMessage"] = "Tour deleted successfully!";

            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour: {TourId}", Tour.TourId);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("./Index");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public TourDto? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id, cancellationToken);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete, id {TourId}", id);
            return RedirectToPage("Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            await _tourService.DeleteAsync(id, cancellationToken);
            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with id {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while deleting the tour.";
            return RedirectToPage("Index");
        }
    }
}

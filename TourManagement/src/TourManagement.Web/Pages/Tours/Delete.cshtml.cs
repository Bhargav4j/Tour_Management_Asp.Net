using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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

    [BindProperty]
    public TourDto? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            Tour = await _tourService.GetByIdAsync(id.Value);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete with ID {TourId}", id);
            ModelState.AddModelError(string.Empty, "Error loading tour. Please try again.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Tour == null || Tour.Id == 0)
        {
            return NotFound();
        }

        try
        {
            await _tourService.DeleteAsync(Tour.Id);

            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", Tour.Id);
            ModelState.AddModelError(string.Empty, "Error deleting tour. Please try again.");
            return Page();
        }
    }
}

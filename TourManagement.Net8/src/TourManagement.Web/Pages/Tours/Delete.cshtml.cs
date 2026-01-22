using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

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

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion, ID: {TourId}", id);
            return RedirectToPage("./Index");
        }
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        try
        {
            await _tourService.DeleteAsync(id);
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the tour.");
            return await OnGetAsync(id);
        }
    }
}

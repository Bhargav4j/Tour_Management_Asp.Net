using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class TourDetailsPageModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<TourDetailsPageModel> _logger;

    public TourDetailsPageModel(ITourService tourService, ILogger<TourDetailsPageModel> logger)
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
            _logger.LogError(ex, "Error loading tour details for ID: {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }
}

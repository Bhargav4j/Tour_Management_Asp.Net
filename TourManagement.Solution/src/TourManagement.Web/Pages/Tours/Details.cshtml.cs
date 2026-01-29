using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class TourDetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<TourDetailsModel> _logger;

    public Tour? Tour { get; set; }

    public TourDetailsModel(ITourService tourService, ILogger<TourDetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetTourByIdAsync(id);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour details for ID {TourId}", id);
            return NotFound();
        }
    }
}

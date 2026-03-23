using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class DetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(ITourService tourService, ILogger<DetailsModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

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

            _logger.LogInformation("Retrieved tour details for ID: {TourId}", id);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour details for ID: {TourId}", id);
            TempData["ErrorMessage"] = "An error occurred while retrieving tour details.";
            return RedirectToPage("./Index");
        }
    }
}

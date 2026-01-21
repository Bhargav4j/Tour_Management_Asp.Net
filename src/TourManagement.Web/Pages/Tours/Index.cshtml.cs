using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

/// <summary>
/// Page model for listing and searching tours
/// </summary>
public class IndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ITourService tourService, ILogger<IndexModel> logger)
    {
        _tourService = tourService ?? throw new ArgumentNullException(nameof(tourService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<Tour> Tours { get; set; } = new List<Tour>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                _logger.LogInformation("Searching tours with term: {SearchTerm}", SearchTerm);
                Tours = await _tourService.SearchAsync(SearchTerm, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Retrieving all tours");
                Tours = await _tourService.GetAllAsync(cancellationToken);
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tours");
            TempData["ErrorMessage"] = "An error occurred while retrieving tours. Please try again.";
            Tours = new List<Tour>();
            return Page();
        }
    }
}

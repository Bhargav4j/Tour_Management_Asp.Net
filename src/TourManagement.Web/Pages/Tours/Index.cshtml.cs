using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

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
    public string? SearchString { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrEmpty(SearchString))
            {
                Tours = await _tourService.SearchAsync(SearchString, cancellationToken);
                _logger.LogInformation("Searched tours with term: {SearchString}", SearchString);
            }
            else
            {
                Tours = await _tourService.GetAllAsync(cancellationToken);
                _logger.LogInformation("Retrieved all tours");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tours");
            TempData["ErrorMessage"] = "An error occurred while retrieving tours.";
            return Page();
        }
    }
}

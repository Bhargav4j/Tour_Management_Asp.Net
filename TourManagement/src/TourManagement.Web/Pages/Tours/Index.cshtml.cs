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
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<Tour>? Tours { get; set; }

    public bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            Tours = await _tourService.GetAllToursAsync(cancellationToken);
            _logger.LogInformation("Retrieved {Count} tours", Tours?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            Tours = Enumerable.Empty<Tour>();
        }
    }
}

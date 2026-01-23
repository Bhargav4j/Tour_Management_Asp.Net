using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class ToursIndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<ToursIndexModel> _logger;

    public ToursIndexModel(ITourService tourService, ILogger<ToursIndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<Tour>? Tours { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Tours = await _tourService.GetAllToursAsync();
            _logger.LogInformation("Retrieved {Count} tours", Tours?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tours");
            Tours = new List<Tour>();
        }
    }
}

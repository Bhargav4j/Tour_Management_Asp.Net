using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

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

    public IEnumerable<TourDto>? Tours { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAdmin => HttpContext.Session.GetString("IsAdmin") == "true";

    public async Task OnGetAsync()
    {
        try
        {
            Tours = await _tourService.GetAllAsync();
            _logger.LogInformation("Retrieved {Count} tours", Tours?.Count() ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tours");
            ErrorMessage = "An error occurred while loading tours.";
        }
    }
}

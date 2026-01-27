using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Services;

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

    public IEnumerable<TourDto> Tours { get; set; } = new List<TourDto>();

    public async Task OnGetAsync()
    {
        try
        {
            Tours = await _tourService.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            ModelState.AddModelError(string.Empty, "Error loading tours. Please try again.");
        }
    }
}

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
        _tourService = tourService;
        _logger = logger;
    }

    public IEnumerable<Tour> Tours { get; set; } = new List<Tour>();

    [TempData]
    public string? Message { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Tours = await _tourService.GetAllToursAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            Message = "Error loading tours. Please try again.";
        }
    }
}

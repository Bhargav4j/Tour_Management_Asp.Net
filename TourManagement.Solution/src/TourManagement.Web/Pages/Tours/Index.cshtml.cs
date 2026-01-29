using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class ToursIndexModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<ToursIndexModel> _logger;

    public IEnumerable<Tour> Tours { get; set; } = new List<Tour>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public ToursIndexModel(ITourService tourService, ILogger<ToursIndexModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public async Task OnGetAsync()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Tours = await _tourService.SearchToursAsync(SearchTerm);
            }
            else
            {
                Tours = await _tourService.GetAllToursAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            ModelState.AddModelError(string.Empty, "Error loading tours");
        }
    }
}

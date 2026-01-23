using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class ToursIndexPageModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<ToursIndexPageModel> _logger;

    public ToursIndexPageModel(ITourService tourService, ILogger<ToursIndexPageModel> logger)
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
            Tours = new List<TourDto>();
        }
    }
}

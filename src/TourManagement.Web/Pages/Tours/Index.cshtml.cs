using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.DTOs;
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

    public IEnumerable<TourDto> Tours { get; set; } = new List<TourDto>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                Tours = await _tourService.SearchAsync(SearchTerm, cancellationToken);
            }
            else
            {
                Tours = await _tourService.GetAllAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tours");
            ModelState.AddModelError(string.Empty, "An error occurred while loading tours.");
        }
    }
}

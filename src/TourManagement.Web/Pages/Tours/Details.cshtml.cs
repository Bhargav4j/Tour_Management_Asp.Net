using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Application.DTOs;
using TourManagement.Application.Interfaces;

namespace TourManagement.Web.Pages.Tours;

public class TourDetailsModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<TourDetailsModel> _logger;

    public TourDetailsModel(ITourService tourService, ILogger<TourDetailsModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    public TourDto? Tour { get; set; }
    public string? UserEmail => HttpContext.Session.GetString("UserEmail");

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            Tour = await _tourService.GetByIdAsync(id);

            if (Tour == null)
            {
                _logger.LogWarning("Tour with ID {TourId} not found", id);
                return NotFound();
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tour with ID {TourId}", id);
            return RedirectToPage("/Tours/Index");
        }
    }
}

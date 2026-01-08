using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

[Authorize]
public class TourDeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<TourDeleteModel> _logger;

    public TourDeleteModel(ITourService tourService, IWebHostEnvironment environment, ILogger<TourDeleteModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public int Id { get; set; }

    public Tour? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            Tour = await _tourService.GetTourByIdAsync(id, cancellationToken);

            if (Tour == null)
            {
                return NotFound();
            }

            Id = Tour.Id;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for deletion: {TourId}", id);
            return RedirectToPage("/Error");
        }
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourService.GetTourByIdAsync(Id, cancellationToken);

            if (tour == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(tour.PicturePath))
            {
                var filePath = Path.Combine(_environment.WebRootPath, "Tour_pics", tour.PicturePath);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            await _tourService.DeleteTourAsync(Id, cancellationToken);

            _logger.LogInformation("Tour deleted successfully: {TourId}", Id);

            return RedirectToPage("/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour: {TourId}", Id);
            return RedirectToPage("/Error");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

public class DeleteModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<DeleteModel> _logger;

    public DeleteModel(ITourService tourService, ILogger<DeleteModel> logger)
    {
        _tourService = tourService;
        _logger = logger;
    }

    [BindProperty]
    public TourViewModel? Tour { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var tour = await _tourService.GetByIdAsync(id.Value);
            if (tour == null)
            {
                return NotFound();
            }

            Tour = MapToViewModel(tour);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for delete with ID {TourId}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (Tour == null || Tour.Id == 0)
        {
            return NotFound();
        }

        try
        {
            await _tourService.DeleteAsync(Tour.Id);

            TempData["SuccessMessage"] = "Tour deleted successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tour with ID {TourId}", Tour.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while deleting the tour. Please try again.");
            return Page();
        }
    }

    private static TourViewModel MapToViewModel(Tour tour)
    {
        return new TourViewModel
        {
            Id = tour.Id,
            Name = tour.Name,
            Place = tour.Place,
            Days = tour.Days,
            Price = tour.Price,
            Locations = tour.Locations,
            TourInfo = tour.TourInfo,
            PictureFileName = tour.PictureFileName
        };
    }
}

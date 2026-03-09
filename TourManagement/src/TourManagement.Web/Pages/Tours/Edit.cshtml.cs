using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;
using TourManagement.Web.ViewModels;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<EditModel> _logger;

    public EditModel(ITourService tourService, IWebHostEnvironment environment, ILogger<EditModel> logger)
    {
        _tourService = tourService;
        _environment = environment;
        _logger = logger;
    }

    [BindProperty]
    public TourViewModel Tour { get; set; } = new();

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
            _logger.LogError(ex, "Error loading tour for edit with ID {TourId}", id);
            return NotFound();
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var tour = MapToEntity(Tour);

            if (Tour.PictureFile != null && Tour.PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{Tour.PictureFile.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Tour.PictureFile.CopyToAsync(fileStream);
                }

                tour.PictureFileName = uniqueFileName;
            }
            else
            {
                tour.PictureFileName = Tour.PictureFileName;
            }

            await _tourService.UpdateAsync(Tour.Id, tour);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID {TourId}", Tour.Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour. Please try again.");
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

    private static Tour MapToEntity(TourViewModel viewModel)
    {
        return new Tour
        {
            Id = viewModel.Id,
            Name = viewModel.Name,
            Place = viewModel.Place,
            Days = viewModel.Days,
            Price = viewModel.Price,
            Locations = viewModel.Locations,
            TourInfo = viewModel.TourInfo,
            ModifiedBy = "Admin"
        };
    }
}

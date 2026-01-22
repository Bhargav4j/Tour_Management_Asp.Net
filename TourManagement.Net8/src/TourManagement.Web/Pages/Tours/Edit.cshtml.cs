using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

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
    public int Id { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Tour name is required")]
    public string TourName { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Place is required")]
    public string Place { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Days is required")]
    [Range(1, 365, ErrorMessage = "Days must be between 1 and 365")]
    public int Days { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Locations is required")]
    public string Locations { get; set; } = string.Empty;

    [BindProperty]
    [Required(ErrorMessage = "Tour information is required")]
    public string TourInfo { get; set; } = string.Empty;

    [BindProperty]
    public IFormFile? PictureFile { get; set; }

    [BindProperty]
    public string? CurrentPicturePath { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        try
        {
            var tour = await _tourService.GetTourByIdAsync(id.Value);

            if (tour == null)
            {
                return NotFound();
            }

            Id = tour.Id;
            TourName = tour.TourName;
            Place = tour.Place;
            Days = tour.Days;
            Price = tour.Price;
            Locations = tour.Locations;
            TourInfo = tour.TourInfo;
            CurrentPicturePath = tour.PicturePath;

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tour for edit with ID: {TourId}", id);
            return RedirectToPage("./Index");
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
            var tour = await _tourService.GetTourByIdAsync(Id);

            if (tour == null)
            {
                return NotFound();
            }

            tour.TourName = TourName;
            tour.Place = Place;
            tour.Days = Days;
            tour.Price = Price;
            tour.Locations = Locations;
            tour.TourInfo = TourInfo;
            tour.ModifiedDate = DateTime.UtcNow;
            tour.ModifiedBy = "Admin";

            if (PictureFile != null && PictureFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "Tour_pics");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + PictureFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await PictureFile.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(tour.PicturePath))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, tour.PicturePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                tour.PicturePath = uniqueFileName;
            }

            await _tourService.UpdateTourAsync(tour);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour with ID: {TourId}", Id);
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }
}

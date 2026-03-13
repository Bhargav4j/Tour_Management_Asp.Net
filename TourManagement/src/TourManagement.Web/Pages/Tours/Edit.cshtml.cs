using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces.Services;

namespace TourManagement.Web.Pages.Tours;

public class EditModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly ILogger<EditModel> _logger;
    private readonly IWebHostEnvironment _environment;

    public EditModel(ITourService tourService, ILogger<EditModel> logger, IWebHostEnvironment environment)
    {
        _tourService = tourService;
        _logger = logger;
        _environment = environment;
    }

    [BindProperty]
    public TourInputModel TourInput { get; set; } = new();

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

            TourInput = new TourInputModel
            {
                Id = tour.Id,
                TourName = tour.TourName,
                Place = tour.Place,
                Days = tour.Days,
                Price = tour.Price,
                Locations = tour.Locations,
                TourInfo = tour.TourInfo,
                ExistingPicturePath = tour.PicturePath
            };

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
            var existingTour = await _tourService.GetByIdAsync(TourInput.Id);
            if (existingTour == null)
            {
                return NotFound();
            }

            existingTour.TourName = TourInput.TourName;
            existingTour.Place = TourInput.Place;
            existingTour.Days = TourInput.Days;
            existingTour.Price = TourInput.Price;
            existingTour.Locations = TourInput.Locations;
            existingTour.TourInfo = TourInput.TourInfo;
            existingTour.ModifiedBy = "Admin";
            existingTour.ModifiedDate = DateTime.UtcNow;

            if (TourInput.Picture != null)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "tours");
                Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = $"{Guid.NewGuid()}_{TourInput.Picture.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await TourInput.Picture.CopyToAsync(fileStream);
                }

                if (!string.IsNullOrEmpty(existingTour.PicturePath))
                {
                    var oldFilePath = Path.Combine(uploadsFolder, existingTour.PicturePath);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                existingTour.PicturePath = uniqueFileName;
            }

            await _tourService.UpdateAsync(TourInput.Id, existingTour);

            TempData["SuccessMessage"] = "Tour updated successfully!";
            return RedirectToPage("./Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while updating the tour.");
            return Page();
        }
    }

    public class TourInputModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Tour Name")]
        public string TourName { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Place { get; set; } = string.Empty;

        [Required]
        [Range(1, 365)]
        public int Days { get; set; }

        [Required]
        [Range(0.01, 1000000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [StringLength(500)]
        public string Locations { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Tour Information")]
        public string TourInfo { get; set; } = string.Empty;

        [Display(Name = "Picture")]
        public IFormFile? Picture { get; set; }

        public string? ExistingPicturePath { get; set; }
    }
}
